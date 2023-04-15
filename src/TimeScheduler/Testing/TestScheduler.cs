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
[Obsolete("Use System.Testing.TestTimeProvider instead. This will allow you to upgrade seamlessly the new TimeProvider API that is part of .NET 8 upon release.")]
public partial class TestScheduler : TimeProvider, ITimeScheduler, IDisposable
{
    internal const uint MaxSupportedTimeout = 0xfffffffe;
    internal const uint UnsignedInfinite = unchecked((uint)-1);

    private readonly ConcurrentDictionary<FutureAction, object?> futureActions = new();
    private readonly ConditionalWeakTable<object, FutureAction> attachedFutureActions = new();
    private bool isDisposed;
    private DateTimeOffset utcNow;

    /// <summary>
    /// Gets a <see cref="DateTimeOffset"/> value whose date and time are set to the current
    /// Coordinated Universal Time (UTC) date and time and whose offset is Zero,
    /// all according to this <see cref="TestScheduler"/>'s notion of time.
    /// </summary>
    /// <remarks>
    /// To advance time, call <see cref="ForwardTime(TimeSpan)"/> or <see cref="SetUtcNow(DateTimeOffset)"/>.
    /// </remarks>
    [Obsolete("Use GetUtcNow() instead. This will allow you to upgrade seamlessly to TimeProvider that will be part of .NET 8.")]
    public DateTimeOffset UtcNow => utcNow;

    /// <summary>
    /// Creates an instance of the <see cref="TestScheduler"/> with
    /// <see cref="DateTimeOffset.UtcNow"/> being the initial value returned by <see cref="GetUtcNow()"/>.
    /// </summary>
    public TestScheduler()
        : this(DateTimeOffset.UtcNow)
    {
    }

    /// <summary>
    /// Creates an instance of the <see cref="TestScheduler"/> with
    /// <paramref name="startDateTime"/> being the initial value returned by <see cref="GetUtcNow()"/>.
    /// </summary>
    /// <param name="startDateTime">The initial date and time <see cref="GetUtcNow()"/> will return.</param>
    public TestScheduler(DateTimeOffset startDateTime)
    {
        utcNow = startDateTime;
    }

    /// <summary>
    /// Gets a <see cref="DateTimeOffset"/> value whose date and time are set to the current
    /// Coordinated Universal Time (UTC) date and time and whose offset is Zero,
    /// all according to this <see cref="TestScheduler"/>'s notion of time.
    /// </summary>
    /// <remarks>
    /// To advance time, call <see cref="ForwardTime(TimeSpan)"/> or <see cref="SetUtcNow(DateTimeOffset)"/>.
    /// </remarks>
    public override DateTimeOffset GetUtcNow() => utcNow;

    /// <summary>
    /// Sets the date and time returned by <see cref="GetUtcNow()"/> to <paramref name="newUtcNew"/> and triggers any
    /// scheduled items that are waiting for time to be forwarded.
    /// </summary>
    /// <param name="newUtcNew">The new UtcNow time.</param>
    /// <exception cref="ArgumentException">If <paramref name="newUtcNew"/> is less than the value returned by <see cref="GetUtcNow()"/>.</exception>
    public virtual void SetUtcNow(DateTimeOffset newUtcNew)
    {
        if (newUtcNew < utcNow)
        {
            throw new ArgumentException("The new UtcNow must be greater than or equal to the current UtcNow.", nameof(newUtcNew));
        }

        utcNow = newUtcNew;
        CompleteDelayedTasks();
    }

    /// <summary>
    /// Forward the date and time represented by <see cref="GetUtcNow()"/>
    /// by the specified <paramref name="time"/>, and triggers any
    /// scheduled items that are waiting for time to be forwarded.
    /// </summary>
    /// <param name="time">The span of time to forward <see cref="GetUtcNow()"/> with.</param>
    /// <exception cref="ArgumentException">If <paramref name="time"/> is negative or zero.</exception>
    public virtual void ForwardTime(TimeSpan time)
    {
        if (time <= TimeSpan.Zero)
        {
            throw new ArgumentException("The timespan to forward time by must be positive.", nameof(time));
        }

        SetUtcNow(utcNow + time);
    }

    /// <summary>
    /// Disposing will cancel all scheduled work items/tasks that are waiting
    /// for time to be forwarded.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposing the scheduler will cancel all scheduled tasks that are waiting
    /// for time to be forwarded.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (isDisposed) return;
        isDisposed = true;

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
            .Where(x => x.CompletionTime <= utcNow)
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
