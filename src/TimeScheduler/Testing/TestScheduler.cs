using System.Collections.Concurrent;

namespace TimeScheduler.Testing;

public sealed partial class TestScheduler : ITimeScheduler, IDisposable
{
    private readonly List<FutureAction> futureActions = new();

    public DateTimeOffset UtcNow { get; private set; }

    public TestScheduler()
    {
        UtcNow = DateTimeOffset.UtcNow;
    }

    public TestScheduler(DateTimeOffset startDateTime)
    {
        UtcNow = startDateTime;
    }

    public Task Delay(TimeSpan delay)
    {
        return Delay(delay, CancellationToken.None);
    }

    public Task Delay(TimeSpan delay, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource();
        RegisterFutureAction(UtcNow + delay, () => tcs.TrySetResult(), () => tcs.TrySetCanceled(), cancellationToken);
        return tcs.Task;
    }

    public PeriodicTimer PeriodicTimer(TimeSpan period)
    {
        return new TestPeriodicTimer(period, this);
    }

    public void ForwardTime(TimeSpan timeSpan)
    {
        UtcNow = UtcNow + timeSpan;
        CompleteDelayedTasks();
    }

    private void CompleteDelayedTasks()
    {
        var tasksToComplete = futureActions
            .Where(x => x.CompletionTime <= UtcNow)
            .OrderBy(x => x.CompletionTime)
            .ToArray();

        foreach (var delayedAction in tasksToComplete)
        {
            delayedAction.Complete();
            futureActions.Remove(delayedAction);
        }
    }

    private void RegisterFutureAction(
        DateTimeOffset completionTime,
        Action complete,
        Action cancel,
        CancellationToken cleanupToken = default)
    {
        var futureAction = new FutureAction(completionTime, complete, cancel);

        cleanupToken.Register(() =>
        {
            cancel();
            futureActions.Remove(futureAction);
        });

        futureActions.Add(futureAction);
    }

    public void Dispose()
    {
        foreach (var delayedAction in futureActions)
        {
            delayedAction.Cancel();
        }
    }

    private sealed class FutureAction
    {
        private readonly Action complete;
        private readonly Action cancel;
        private int completed;

        public DateTimeOffset CompletionTime { get; }

        public FutureAction(DateTimeOffset completionTime, Action complete, Action cancel)
        {
            CompletionTime = completionTime;
            this.complete = complete;
            this.cancel = cancel;
        }

        public void Complete()
        {
            // Ensure that complete/cancel is only being called once.
            if (Interlocked.CompareExchange(ref completed, 1, 0) == 0)
            {
                complete();
            }
        }

        public void Cancel()
        {
            // Ensure that cancel/cancel is only being called once.
            if (Interlocked.CompareExchange(ref completed, 2, 0) == 0)
            {
                cancel();
            }
        }
    }
}
