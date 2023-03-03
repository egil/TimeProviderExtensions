using System.Collections.Concurrent;

namespace TimeScheduler.Testing;

/// <summary>
/// Represents a test implementation of a <see cref="ITimeScheduler"/>,
/// where time stands still until a call to <see cref="ForwardTime(TimeSpan)"/>
/// is called.
/// </summary>
/// <remarks>
/// Learn more at <see href="https://github.com/egil/TimeScheduler"/>.
/// </remarks>
public sealed partial class TestScheduler : ITimeScheduler, IDisposable
{
    private readonly ConcurrentDictionary<FutureAction, object?> futureActions = new();

    /// <summary>
    /// Returns a DateTimeOffset representing the <see cref="TestScheduler"/>'s
    /// current date and time.
    /// </summary>
    /// <remarks>
    /// To advance time, call <see cref="ForwardTime(TimeSpan)"/>.
    /// </remarks>
    public DateTimeOffset UtcNow { get; private set; }

    /// <summary>
    /// Creates an instance of the <see cref="TestScheduler"/> with
    /// <see cref="UtcNow"/> set to <see cref="DateTimeOffset.UtcNow"/>.
    /// </summary>
    public TestScheduler()
    {
        UtcNow = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Creates an instance of the <see cref="TestScheduler"/> with
    /// <see cref="UtcNow"/> set to <paramref name="startDateTime"/>.
    /// </summary>
    /// <param name="startDateTime">The date and time to use for <see cref="UtcNow"/> initially.</param>
    public TestScheduler(DateTimeOffset startDateTime)
    {
        UtcNow = startDateTime;
    }

    /// <summary>
    /// Forward the date and time represented by <see cref="UtcNow"/>
    /// by the specified <paramref name="time"/>.
    /// </summary>
    /// <param name="time">The span of time to forward <see cref="UtcNow"/> with.</param>
    /// <exception cref="ArgumentException">If <paramref name="time"/> is negative or zero.</exception>
    public void ForwardTime(TimeSpan time)
    {
        if (time <= TimeSpan.Zero)
        {
            throw new ArgumentException("The timespan to forward time by must be positive.", nameof(time));
        }

        UtcNow = UtcNow + time;
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

    /// <summary>
    /// Disposing the scheduler will cancel all scheduled tasks that are waiting
    /// for time to be forwarded.
    /// </summary>
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
