using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace TimeProviderExtensions;

/// <summary>
/// A implementaiton of a <see cref="ITimer"/> whose callbacks are scheduled via a <see cref="ManualTimeProvider"/>.
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
public class ManualTimer : ITimer
{
    private const uint MaxSupportedTimeout = 0xfffffffe;

    private readonly object lockObject = new();
    private ManualTimerScheduler? scheduler;

    /// <summary>
    /// Gets the next time the timer callback will be invoked, or <c>null</c> if the timer is inactive.
    /// </summary>
    public DateTimeOffset? CallbackTime => scheduler?.CallbackTime;

    /// <summary>
    /// Gets the <see cref="TimeSpan"/> representing the amount of time to delay before invoking the callback method specified when the <see cref="ITimer"/> was constructed.
    /// </summary>
    public TimeSpan DueTime { get; private set; }

    /// <summary>
    /// Gets the time interval between invocations of the callback method specified when the Timer was constructed.
    /// If set to <see cref="Timeout.InfiniteTimeSpan"/> periodic signaling is disabled.
    /// </summary>
    public TimeSpan Period { get; private set; }

    /// <summary>
    /// Creates an instance of the <see cref="ManualTimer"/>. No callbacks are scheduled during construction. Call <see cref="Change(TimeSpan, TimeSpan)"/> to schedule invocations of <paramref name="callback"/> using the provided <paramref name="timeProvider"/>.
    /// </summary>
    /// <param name="callback">
    /// A delegate representing a method to be executed when the timer fires. The method specified for callback should be reentrant,
    /// as it may be invoked simultaneously on two threads if the timer fires again before or while a previous callback is still being handled.
    /// </param>
    /// <param name="state">An object to be passed to the <paramref name="callback"/>. This may be null.</param>
    /// <param name="timeProvider">The <see cref="ManualTimeProvider"/> which is used to schedule invocations of the <paramref name="callback"/> with.</param>
    protected internal ManualTimer(TimerCallback callback, object? state, ManualTimeProvider timeProvider)
    {
        scheduler = new ManualTimerScheduler(timeProvider, callback, state);
    }

    /// <summary>Changes the start time and the interval between method invocations for a timer, using <see cref="TimeSpan"/> values to measure time intervals.</summary>
    /// <param name="dueTime">
    /// A <see cref="TimeSpan"/> representing the amount of time to delay before invoking the callback method specified when the <see cref="ITimer"/> was constructed.
    /// Specify <see cref="Timeout.InfiniteTimeSpan"/> to prevent the timer from restarting. Specify <see cref="TimeSpan.Zero"/> to restart the timer immediately.
    /// </param>
    /// <param name="period">
    /// The time interval between invocations of the callback method specified when the Timer was constructed.
    /// Specify <see cref="Timeout.InfiniteTimeSpan"/> to disable periodic signaling.
    /// </param>
    /// <returns><see langword="true"/> if the timer was successfully updated; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="dueTime"/> or <paramref name="period"/> parameter, in milliseconds, is less than -1 or greater than 4294967294.</exception>
    /// <remarks>
    /// It is the responsibility of the implementer of the ITimer interface to ensure thread safety.
    /// </remarks>
    public virtual bool Change(TimeSpan dueTime, TimeSpan period)
    {
        ValidateTimeSpanRange(dueTime);
        ValidateTimeSpanRange(period);

        lock (lockObject)
        {
            if (scheduler is null)
            {
                return false;
            }

            DueTime = dueTime;
            Period = period;

            scheduler.Change(dueTime, period);

            return true;
        }
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        if (scheduler is null)
            return "Timer is disposed.";

        var status = scheduler.CallbackTime.HasValue
            ? $"Next callback: {scheduler.CallbackTime.Value.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture)}"
            : "Timer is disabled";

        return $"{status}. DueTime: {(DueTime == Timeout.InfiniteTimeSpan ? "Infinite" : DueTime)}. Period: {(Period == Timeout.InfiniteTimeSpan ? "Infinite" : Period)}.";
    }

    /// <summary>
    /// The finalizer exists in case the timer is not disposed explicitly by the user.
    /// </summary>
    ~ManualTimer() => Dispose(false);

    /// <summary>
    /// Disposes of the <see cref="ManualTimer"/> and removes any scheduled callbacks from the <see cref="ManualTimeProvider"/>.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes of the <see cref="ManualTimer"/> and removes any scheduled callbacks from the <see cref="ManualTimeProvider"/>.
    /// </summary>
    public ValueTask DisposeAsync()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
#if NET5_0_OR_GREATER
        return ValueTask.CompletedTask;
#else
        return default;
#endif
    }

    /// <summary>
    /// Disposes of the <see cref="ManualTimer"/> and removes any scheduled callbacks from the <see cref="ManualTimeProvider"/>.
    /// </summary>
    /// <remarks>
    /// If this method is overridden, it should always be called by the overriding method.
    /// </remarks>
    protected virtual void Dispose(bool disposing)
    {
        lock (lockObject)
        {
            if (scheduler is null)
            {
                return;
            }

            scheduler.Cancel();
            scheduler = null;
        }
    }

    private static void ValidateTimeSpanRange(TimeSpan time, [CallerArgumentExpression(nameof(time))] string? parameter = null)
    {
        long tm = (long)time.TotalMilliseconds;
        if (tm < -1)
        {
            throw new ArgumentOutOfRangeException(parameter, $"{parameter}.TotalMilliseconds must be greater than -1.");
        }

        if (tm > MaxSupportedTimeout)
        {
            throw new ArgumentOutOfRangeException(parameter, $"{parameter}.TotalMilliseconds must be less than than {MaxSupportedTimeout}.");
        }
    }
}
