using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Time.Testing;
using static Microsoft.Extensions.Time.Testing.FakeTimeProviderTimer;

namespace TimeProviderExtensions;

/// <summary>
/// Represents a test implementation of a <see cref="TimeProvider"/>,
/// where time stands still until you call <see cref="ForwardTime(TimeSpan)"/>
/// or <see cref="SetUtcNow(DateTimeOffset)"/>.
/// </summary>
/// <remarks>
/// Learn more at <see href="https://github.com/egil/TimeProviderExtensions"/>.
/// </remarks>
public class ManualTimeProvider : TimeProvider
{
    internal const uint MaxSupportedTimeout = 0xfffffffe;
    internal const uint UnsignedInfinite = unchecked((uint)-1);

    private readonly List<ManualTimerScheduledCallback> futureCallbacks = new();
    private DateTimeOffset utcNow;
    private TimeZoneInfo localTimeZone = TimeZoneInfo.Utc;

    /// <summary>
    /// Gets the frequency of <see cref="GetTimestamp"/> of high-frequency value per second.
    /// </summary>
    /// <remarks>
    /// This implementation bases timestamp on <see cref="DateTimeOffset.UtcTicks"/>, which is 10,000 ticks per millisecond,
    /// since the progression of time is represented by the date and time returned from <see cref="GetUtcNow()" />.
    /// </remarks>
    public override long TimestampFrequency { get; } = 10_000_000;

    /// <inheritdoc />
    public override TimeZoneInfo LocalTimeZone => localTimeZone;

    /// <summary>
    /// Creates an instance of the <see cref="ManualTimeProvider"/> with
    /// <see cref="DateTimeOffset.UtcNow"/> being the initial value returned by <see cref="GetUtcNow()"/>.
    /// </summary>
    public ManualTimeProvider()
        : this(System.GetUtcNow())
    /// <param name="localTimeZone">Optional local time zone to use during testing. Defaults to <see cref="TimeZoneInfo.Utc"/>.</param>
    public ManualTimeProvider(TimeZoneInfo? localTimeZone = null)
        : this(Epoch)
    {
        
        this.localTimeZone = localTimeZone ?? TimeZoneInfo.Utc;
    }

    /// <summary>
    /// Creates an instance of the <see cref="ManualTimeProvider"/> with
    /// <paramref name="startDateTime"/> being the initial value returned by <see cref="GetUtcNow()"/>.
    /// </summary>
    /// <param name="startDateTime">The initial date and time <see cref="GetUtcNow()"/> will return.</param>
    public ManualTimeProvider(DateTimeOffset startDateTime)
    {
        utcNow = startDateTime;
    }

    /// <summary>
    /// Creates an instance of the <see cref="ManualTimeProvider"/> with
    /// <paramref name="startDateTime"/> being the initial value returned by <see cref="GetUtcNow()"/>.
    /// </summary>
    /// <param name="startDateTime">The initial date and time <see cref="GetUtcNow()"/> will return.</param>
    /// <param name="localTimeZone">Optional local time zone to use during testing. Defaults to <see cref="TimeZoneInfo.Utc"/>.</param>
    public ManualTimeProvider(DateTimeOffset startDateTime, TimeZoneInfo? localTimeZone = null)
    {
        utcNow = startDateTime;
        this.localTimeZone = localTimeZone ?? TimeZoneInfo.Utc;
    }

    /// <summary>
    /// Gets the current high-frequency value designed to measure small time intervals with high accuracy in the timer mechanism.
    /// </summary>
    /// <returns>A long integer representing the high-frequency counter value of the underlying timer mechanism. </returns>
    /// <remarks>
    /// This implementation bases timestamp on <see cref="DateTimeOffset.UtcTicks"/>,
    /// since the progression of time is represented by the date and time returned from <see cref="GetUtcNow()" />.
    /// </remarks>
    public override long GetTimestamp() => GetUtcNow().UtcTicks;

    /// <summary>
    /// Gets a <see cref="DateTimeOffset"/> value whose date and time are set to the current
    /// Coordinated Universal Time (UTC) date and time and whose offset is Zero,
    /// all according to this <see cref="ManualTimeProvider"/>'s notion of time.
    /// </summary>
    /// <remarks>
    /// To advance time, call <see cref="ForwardTime(TimeSpan)"/> or <see cref="SetUtcNow(DateTimeOffset)"/>.
    /// </remarks>
    public override DateTimeOffset GetUtcNow() => utcNow;

    /// <summary>Creates a new <see cref="ITimer"/> instance, using <see cref="TimeSpan"/> values to measure time intervals.</summary>
    /// <param name="callback">
    /// A delegate representing a method to be executed when the timer fires. The method specified for callback should be reentrant,
    /// as it may be invoked simultaneously on two threads if the timer fires again before or while a previous callback is still being handled.
    /// </param>
    /// <param name="state">An object to be passed to the <paramref name="callback"/>. This may be null.</param>
    /// <param name="dueTime">The amount of time to delay before <paramref name="callback"/> is invoked. Specify <see cref="Timeout.InfiniteTimeSpan"/> to prevent the timer from starting. Specify <see cref="TimeSpan.Zero"/> to start the timer immediately.</param>
    /// <param name="period">The time interval between invocations of <paramref name="callback"/>. Specify <see cref="Timeout.InfiniteTimeSpan"/> to disable periodic signaling.</param>
    /// <returns>
    /// The newly created <see cref="ITimer"/> instance.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="callback"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The number of milliseconds in the value of <paramref name="dueTime"/> or <paramref name="period"/> is negative and not equal to <see cref="Timeout.Infinite"/>, or is greater than <see cref="int.MaxValue"/>.</exception>
    /// <remarks>
    /// <para>
    /// The delegate specified by the callback parameter is invoked once after <paramref name="dueTime"/> elapses, and thereafter each time the <paramref name="period"/> time interval elapses.
    /// </para>
    /// <para>
    /// If <paramref name="dueTime"/> is zero, the callback is invoked immediately. If <paramref name="dueTime"/> is -1 milliseconds, <paramref name="callback"/> is not invoked; the timer is disabled,
    /// but can be re-enabled by calling the <see cref="ITimer.Change"/> method.
    /// </para>
    /// <para>
    /// If <paramref name="period"/> is 0 or -1 milliseconds and <paramref name="dueTime"/> is positive, <paramref name="callback"/> is invoked once; the periodic behavior of the timer is disabled,
    /// but can be re-enabled using the <see cref="ITimer.Change"/> method.
    /// </para>
    /// <para>
    /// The return <see cref="ITimer"/> instance will be implicitly rooted while the timer is still scheduled.
    /// </para>
    /// <para>
    /// <see cref="CreateTimer"/> captures the <see cref="ExecutionContext"/> and stores that with the <see cref="ITimer"/> for use in invoking <paramref name="callback"/>
    /// each time it's called. That capture can be suppressed with <see cref="ExecutionContext.SuppressFlow"/>.
    /// </para>
    /// <para>
    /// To advance time, call <see cref="ForwardTime(TimeSpan)"/> or <see cref="SetUtcNow(DateTimeOffset)"/>.
    /// </para>
    /// </remarks>
    public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
        => new ManualTimer(callback, state, dueTime, period, this);

    /// <summary>
    /// Forward the date and time represented by <see cref="GetUtcNow()"/>
    /// by the specified <paramref name="delta"/>, and triggers any
    /// scheduled items that are waiting for time to be forwarded.
    /// </summary>
    /// <param name="delta">The span of time to forward <see cref="GetUtcNow()"/> with.</param>
    /// <exception cref="ArgumentException">If <paramref name="delta"/> is negative or zero.</exception>
    public void ForwardTime(TimeSpan delta)
    {
        if (delta <= TimeSpan.Zero)
            throw new ArgumentException("The timespan to forward time by must be positive.", nameof(delta));

        SetUtcNow(utcNow + delta);
    }

    /// <summary>
    /// Sets the local time zone.
    /// </summary>
    /// <param name="localTimeZone">The local time zone.</param>
    public void SetLocalTimeZone(TimeZoneInfo localTimeZone)
    {
        this.localTimeZone = localTimeZone;
    }

    /// <summary>
    /// Advance the date and time represented by <see cref="GetUtcNow()"/>
    /// by the specified <paramref name="delta"/>, and triggers any
    /// scheduled items that are waiting for time to be forwarded.
    /// </summary>
    /// <param name="delta">The amount of time to advance the clock by.</param>
    public void Advance(TimeSpan delta)
    {
        SetUtcNow(utcNow + delta);
    }

    /// <summary>
    /// Advance the date and time represented by <see cref="GetUtcNow()"/>
    /// by one millisecond, and triggers any scheduled items that are waiting for time to be forwarded.
    /// </summary>
    public void Advance()
        => Advance(TimeSpan.FromMilliseconds(1));

    /// <summary>
    /// Sets the date and time returned by <see cref="GetUtcNow()"/> to <paramref name="newUtcNew"/> and triggers any
    /// scheduled items that are waiting for time to be forwarded.
    /// </summary>
    /// <param name="newUtcNew">The new UtcNow time.</param>
    /// <exception cref="ArgumentException">If <paramref name="newUtcNew"/> is less than the value returned by <see cref="GetUtcNow()"/>.</exception>
    public void SetUtcNow(DateTimeOffset newUtcNew)
    {
        if (newUtcNew < utcNow)
            throw new ArgumentException("The new UtcNow must be greater than or equal to the current UtcNow.", nameof(newUtcNew));

        while (utcNow <= newUtcNew && TryGetNext(newUtcNew) is ManualTimerScheduledCallback mtsc)
        {
            utcNow = mtsc.CallbackTime;
            mtsc.Timer.TimerElapsed();
        }

        utcNow = newUtcNew;

        ManualTimerScheduledCallback? TryGetNext(DateTimeOffset targetUtcNow)
        {
            lock (futureCallbacks)
            {
                if (futureCallbacks.Count > 0 && futureCallbacks[0].CallbackTime <= targetUtcNow)
                {
                    var callback = futureCallbacks[0];
                    futureCallbacks.RemoveAt(0);
                    return callback;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Returns a string representation this clock's current time.
    /// </summary>
    /// <returns>A string representing the clock's current time.</returns>
    public override string ToString()
        => GetUtcNow().ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);

    private void ScheduleCallback(ManualTimer timer, TimeSpan waitTime)
    {
        lock (futureCallbacks)
        {
            var mtsc = new ManualTimerScheduledCallback(timer, utcNow + waitTime);
            var insertPosition = futureCallbacks.FindIndex(x => x.CallbackTime > mtsc.CallbackTime);

            if (insertPosition == -1)
                futureCallbacks.Add(mtsc);
            else
            {
                futureCallbacks.Insert(insertPosition, mtsc);
            }
        }
    }

    private void RemoveCallback(ManualTimer timer)
    {
        lock (futureCallbacks)
        {
            var existingIndexOf = futureCallbacks.FindIndex(0, x => ReferenceEquals(x.Timer, timer));
            if (existingIndexOf >= 0)
                futureCallbacks.RemoveAt(existingIndexOf);
        }
    }

    private readonly struct ManualTimerScheduledCallback :
        IEqualityComparer<ManualTimerScheduledCallback>,
        IComparable<ManualTimerScheduledCallback>
    {
        public readonly ManualTimer Timer { get; }
        public readonly DateTimeOffset CallbackTime { get; }

        public ManualTimerScheduledCallback(ManualTimer timer, DateTimeOffset callbackTime)
        {
            Timer = timer;
            CallbackTime = callbackTime;
        }

        public readonly bool Equals(ManualTimerScheduledCallback x, ManualTimerScheduledCallback y)
            => ReferenceEquals(x.Timer, y.Timer);

        public readonly int GetHashCode(ManualTimerScheduledCallback obj)
            => Timer.GetHashCode();

        public readonly int CompareTo(ManualTimerScheduledCallback other)
            => Comparer<DateTimeOffset>.Default.Compare(CallbackTime, other.CallbackTime);
    }

    private sealed class ManualTimer : ITimer
    {
        private readonly ManualTimeProvider owner;
        private bool isDisposed;
        private bool running;

        private TimeSpan currentDueTime;
        private TimeSpan currentPeriod;
        private object? state;
        private TimerCallback? callback;

        public ManualTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period, ManualTimeProvider owner)
        {
            ValidateTimeSpanRange(dueTime);
            ValidateTimeSpanRange(period);
            this.callback = callback;
            this.state = state;
            currentDueTime = dueTime;
            currentPeriod = period;
            this.owner = owner;

            if (currentDueTime != Timeout.InfiniteTimeSpan)
                ScheduleCallback(dueTime);
        }

        public bool Change(TimeSpan dueTime, TimeSpan period)
        {
            ValidateTimeSpanRange(dueTime);
            ValidateTimeSpanRange(period);

            if (running)
                owner.RemoveCallback(this);

            currentDueTime = dueTime;
            currentPeriod = period;

            if (currentDueTime != Timeout.InfiniteTimeSpan)
                ScheduleCallback(dueTime);

            return true;
        }

        public void Dispose()
        {
            if (isDisposed)
                return;

            isDisposed = true;

            if (running)
                owner.RemoveCallback(this);

            callback = null;
            state = null;
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
#if NETSTANDARD2_0
            return new ValueTask(Task.CompletedTask);
#else
            return ValueTask.CompletedTask;
#endif
        }

        internal void TimerElapsed()
        {
            if (isDisposed)
                return;

            running = false;

            callback?.Invoke(state);

            if (currentPeriod != Timeout.InfiniteTimeSpan)
                ScheduleCallback(currentPeriod);
        }

        private void ScheduleCallback(TimeSpan waitTime)
        {
            if (isDisposed)
                return;

            running = true;
            owner.ScheduleCallback(this, waitTime);
        }

        private static void ValidateTimeSpanRange(TimeSpan time, [CallerArgumentExpression("time")] string? parameter = null)
        {
            long tm = (long)time.TotalMilliseconds;
            if (tm < -1)
                throw new ArgumentOutOfRangeException(parameter, $"{parameter}.TotalMilliseconds must be greater than -1.");

            if (tm > MaxSupportedTimeout)
                throw new ArgumentOutOfRangeException(parameter, $"{parameter}.TotalMilliseconds must be less than than {MaxSupportedTimeout}.");
        }
    }
}
