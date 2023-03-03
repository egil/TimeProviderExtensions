using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

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
    internal const uint MaxSupportedTimeout = 0xfffffffe;
    internal const uint UnsignedInfinite = unchecked((uint)-1);

    private readonly ConcurrentDictionary<FutureAction, object?> futureActions = new();
    private readonly ConditionalWeakTable<object, FutureAction> attachedFutureActions = new();

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

    /// <summary>
    /// Disposing the scheduler will cancel all scheduled tasks that are waiting
    /// for time to be forwarded.
    /// </summary>
    public void Dispose()
    {
        attachedFutureActions.Clear();

        foreach (var futureAction in futureActions.Keys)
        {
            futureAction.Cleanup();
        }

        futureActions.Clear();
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
        CancellationToken cleanupToken)
    {
        var futureAction = new FutureAction(
            completionTime,
            complete,
            cancel,
            RemoveFutureAction,
            cleanupToken);

        // If the cleanup token is already canceled, the cancel        
        // action will run immediately.
        if (!cleanupToken.IsCancellationRequested && !futureAction.IsCompleted)
        {
            futureActions.TryAdd(futureAction, null);
        }

        return futureAction;
    }

    private FutureAction RegisterAttachedFutureAction(
        DateTimeOffset completionTime,
        object owner,
        Action complete,
        Action cancel,
        CancellationToken cleanupToken)
    {
        lock (attachedFutureActions)
        {
            if (attachedFutureActions.TryGetValue(owner, out var futureAction))
            {
                futureAction.Cleanup();
            }

            futureAction = RegisterFutureAction(
                completionTime,
                complete,
                cancel,
                cleanupToken);

            attachedFutureActions.AddOrUpdate(owner, futureAction);

            return futureAction;
        }
    }

    private void RemoveFutureAction(FutureAction futureAction)
    {
        futureActions.TryRemove(futureAction, out var _);
    }

    private static void ThrowIfInvalidUnspportedTimespan(TimeSpan timespan, [CallerArgumentExpression("timespan")] string? paramName = null)
    {
        long totalMilliseconds = (long)timespan.TotalMilliseconds;
        if (totalMilliseconds < -1 || totalMilliseconds > MaxSupportedTimeout)
        {
            throw new ArgumentOutOfRangeException(paramName, $"The value needs to translate in milliseconds to -1 (signifying an infinite timeout), 0, or a positive integer less than or equal to the maximum allowed timer duration ({MaxSupportedTimeout:N0}).");
        }
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
                Cleanup();
            }
        }

        public void Cancel()
        {
            // Ensure that complete/cancel is only being called once.
            if (Interlocked.CompareExchange(ref completed, 2, 0) == 0)
            {
                cancel();
                Cleanup();
            }
        }

        public void Cleanup()
        {
            cleanup(this);
            registration.Dispose();
        }
    }
}
