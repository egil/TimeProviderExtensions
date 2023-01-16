using System.Collections.Concurrent;

namespace Scheduler.Testing;

public sealed partial class TestScheduler : ITimeScheduler, IDisposable
{
    private readonly List<DelayedAction> delayedActions = new();

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
        RegisterDelayedTask(UtcNow + delay, () => tcs.TrySetResult(), () => tcs.TrySetCanceled(), cancellationToken);
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
        var tasksToComplete = delayedActions
            .Where(x => x.CompletionTime <= UtcNow)
            .OrderBy(x => x.CompletionTime)
            .ToArray();

        foreach (var pair in tasksToComplete)
        {
            pair.Complete();
            delayedActions.Remove(pair);
        }
    }

    private void RegisterDelayedTask(
        DateTimeOffset completionTime,
        Action complete,
        Action cancel,
        CancellationToken cleanupToken = default)
    {
        var delayedAction = new DelayedAction(completionTime, complete, cancel);

        cleanupToken.Register(() =>
        {
            cancel();
            delayedActions.Remove(delayedAction);
        });

        delayedActions.Add(delayedAction);
    }

    public void Dispose()
    {
        foreach (var delayedAction in delayedActions)
        {
            delayedAction.Cancel();
        }
    }

    private sealed class DelayedAction
    {
        private readonly Action complete;
        private readonly Action cancel;
        private int completed;

        public DateTimeOffset CompletionTime { get; }

        public DelayedAction(DateTimeOffset completionTime, Action complete, Action cancel)
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
