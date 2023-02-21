using System.Collections.Concurrent;

namespace TimeScheduler.Testing;

public sealed partial class TestScheduler : ITimeScheduler, IDisposable
{
    private readonly ConcurrentDictionary<FutureAction, object?> futureActions = new();

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
            .Keys
            .Where(x => x.CompletionTime <= UtcNow)
            .OrderBy(x => x.CompletionTime);

        foreach (var delayedAction in tasksToComplete)
        {
            futureActions.TryRemove(delayedAction, out var _);
            delayedAction.Complete();
        }
    }

    private FutureAction RegisterFutureAction(
        DateTimeOffset completionTime,
        Action complete,
        Action cancel,
        CancellationToken cleanupToken = default)
    {
        var futureAction = new FutureAction(
            completionTime,
            complete,
            cancel,
            futureAction => futureActions.TryRemove(futureAction, out var _),
            cleanupToken);

        // If the cleanup token is already canceled, the cancel        
        // action will run immediately.
        if (!cleanupToken.IsCancellationRequested && !futureAction.IsCompleted)
        {
            futureActions.TryAdd(futureAction, null);
        }
        
        return futureAction;
    }

    public void Dispose()
    {
        foreach (var futureAction in futureActions.Keys)
        {
            futureAction.Cancel();
        }

        futureActions.Clear();
    }

    // TODO: Would perf improve if this was a ref struct?
    private sealed class FutureAction
    {
        private readonly Action complete;
        private readonly Action cancel;
        private readonly Action<FutureAction> cleanup;
        private readonly CancellationTokenRegistration registration;
        private int completed;

        public bool IsCompleted => completed != 0;

        public DateTimeOffset CompletionTime { get; }

        public FutureAction(
            DateTimeOffset completionTime,
            Action complete,
            Action cancel,
            Action<FutureAction> cleanup,
            in CancellationToken cancellationToken)
        {
            CompletionTime = completionTime;
            this.complete = complete;
            this.cancel = cancel;
            this.cleanup = cleanup;
            registration = cancellationToken.UnsafeRegister(static (state, token) =>
            {
                var futureAction = (FutureAction)state!;
                futureAction.Cancel();
            }, this);
        }

        public void Complete()
        {
            // Ensure that complete/cancel is only being called once.
            if (Interlocked.CompareExchange(ref completed, 1, 0) == 0)
            {
                complete();
                cleanup(this);
                registration.Dispose();
            }
        }

        public void Cancel()
        {
            // Ensure that complete/cancel is only being called once.
            if (Interlocked.CompareExchange(ref completed, 2, 0) == 0)
            {
                cancel();
                cleanup(this);
                registration.Dispose();
            }
        }
    }
}
