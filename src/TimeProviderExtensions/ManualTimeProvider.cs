using System.Diagnostics;
using System.Globalization;

namespace TimeProviderExtensions;

/// <summary>
/// Represents a synthetic time provider that can be used to enable deterministic behavior in tests.
/// </summary>
/// <remarks>
/// Learn more at <see href="https://github.com/egil/TimeProviderExtensions"/>.
/// </remarks>
[DebuggerDisplay("UtcNow: {ToString(),nq}. Active timers: {ActiveTimers}. Auto advance amount: {AutoAdvanceAmount,nq}.")]
public class ManualTimeProvider : TimeProvider
{
    internal const uint MaxSupportedTimeout = 0xfffffffe;
    internal const uint UnsignedInfinite = unchecked((uint)-1);
    internal static readonly DateTimeOffset DefaultStartDateTime = new(2000, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);

    private readonly List<ManualTimerScheduler> callbacks = new();
    private DateTimeOffset utcNow;
    private TimeZoneInfo localTimeZone;
    private TimeSpan autoAdvanceAmount = TimeSpan.Zero;

    /// <summary>
    /// Gets the number of active <see cref="ManualTimer"/>, that have callbacks that are scheduled to be triggered at some point in the future.
    /// </summary>
    public int ActiveTimers => callbacks.Count;

    /// <summary>
    /// Gets the starting date and time for this provider.
    /// </summary>
    public DateTimeOffset Start { get; init; }

    /// <summary>
    /// Gets or sets the amount of time by which time advances whenever the clock is read via <see cref="GetUtcNow"/>.
    /// </summary>
    /// <remarks>
    /// Set to <see cref="TimeSpan.Zero"/> to disable auto advance. The default value is <see cref="TimeSpan.Zero"/>.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when set to a value than <see cref="TimeSpan.Zero"/>.</exception>
    public TimeSpan AutoAdvanceAmount
    {
        get => autoAdvanceAmount;
        set
        {
            if (value < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(AutoAdvanceAmount), "Auto advance amount cannot be less than zero. ");
            }

            autoAdvanceAmount = value;
        }
    }

    /// <summary>
    /// Gets the amount by which the value from <see cref="GetTimestamp"/> increments per second.
    /// </summary>
    /// <remarks>
    /// This is fixed to the value of <see cref="TimeSpan.TicksPerSecond"/>.
    /// </remarks>
    public override long TimestampFrequency { get; } = TimeSpan.TicksPerSecond;

    /// <inheritdoc />
    public override TimeZoneInfo LocalTimeZone => localTimeZone;

    /// <summary>
    /// Initializes a new instance of the <see cref="ManualTimeProvider"/> class.
    /// </summary>
    /// <remarks>
    /// This creates a provider whose time is initially set to midnight January 1st 2000 and
    /// with the local time zone set to <see cref="TimeZoneInfo.Utc"/>.
    /// The provider is set to not automatically advance time each time it is read.
    /// </remarks>
    public ManualTimeProvider()
        : this(DefaultStartDateTime, TimeZoneInfo.Utc)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ManualTimeProvider"/> class.
    /// </summary>
    /// <param name="startDateTime">The initial time and date reported by the provider.</param>
    /// <remarks>
    /// The local time zone set to <see cref="TimeZoneInfo.Utc"/>.
    /// The provider is set to not automatically advance time each time it is read.
    /// </remarks>
    public ManualTimeProvider(DateTimeOffset startDateTime)
        : this(startDateTime, TimeZoneInfo.Utc)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ManualTimeProvider"/> class.
    /// </summary>
    /// <param name="startDateTime">The initial time and date reported by the provider.</param>
    /// <param name="localTimeZone">Optional local time zone to use during testing. Defaults to <see cref="TimeZoneInfo.Utc"/>.</param>
    /// <remarks>
    /// The provider is set to not automatically advance time each time it is read.
    /// </remarks>
    public ManualTimeProvider(DateTimeOffset startDateTime, TimeZoneInfo localTimeZone)
    {
        utcNow = startDateTime;
        this.localTimeZone = localTimeZone;
        Start = startDateTime;
    }

    /// <summary>
    /// Gets the current high-frequency value designed to measure small time intervals with high accuracy in the timer mechanism.
    /// </summary>
    /// <returns>A long integer representing the high-frequency counter value of the underlying timer mechanism.</returns>
    /// <remarks>
    /// This implementation bases timestamp on <see cref="DateTimeOffset.UtcTicks"/>,
    /// since the progression of time is represented by the date and time returned from <see cref="GetUtcNow()" />.
    /// </remarks>
    public override long GetTimestamp() => utcNow.UtcTicks;

    /// <summary>
    /// Gets a <see cref="DateTimeOffset"/> value whose date and time are set to the current
    /// Coordinated Universal Time (UTC) date and time and whose offset is Zero,
    /// all according to this <see cref="ManualTimeProvider"/>'s notion of time.
    /// </summary>
    /// <remarks>
    /// If <see cref="AutoAdvanceAmount"/> is greater than <see cref="TimeSpan.Zero"/>, calling this
    /// method will move time forward by the amount specified by <see cref="AutoAdvanceAmount"/>.
    /// The <see cref="DateTimeOffset"/> returned from this method will reflect the time before
    /// the auto advance was applied, if any.
    /// </remarks>
    public override DateTimeOffset GetUtcNow()
    {
        DateTimeOffset result;

        lock (callbacks)
        {
            result = utcNow;
            Advance(AutoAdvanceAmount);
        }

        return result;
    }

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
    /// To move time forward for the returned <see cref="ITimer"/>, call <see cref="Advance(TimeSpan)"/> or <see cref="SetUtcNow(DateTimeOffset)"/> on this time provider.
    /// </para>
    /// </remarks>
    public sealed override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
    {
        var result = CreateManualTimer(callback, state, this);
        result.Change(dueTime, period);
        return result;
    }

    /// <summary>
    /// Creates an instance of a <see cref="ManualTimer"/>. This method is called by <see cref="CreateTimer(TimerCallback, object?, TimeSpan, TimeSpan)"/>.
    /// </summary>
    /// <remarks>
    /// Override this methods to return a custom implementation of <see cref="ManualTimer"/>. This also allows for intercepting and wrapping
    /// the provided timer <paramref name="callback"/> and <paramref name="state"/>, enabling more advanced testing scenarioes.
    /// </remarks>
    /// <param name="callback">
    /// A delegate representing a method to be executed when the timer fires. The method specified for callback should be reentrant,
    /// as it may be invoked simultaneously on two threads if the timer fires again before or while a previous callback is still being handled.
    /// </param>
    /// <param name="state">An object to be passed to the <paramref name="callback"/>. This may be null.</param>
    /// <param name="timeProvider">The <see cref="ManualTimeProvider"/> which is used to schedule invocations of the <paramref name="callback"/> with.</param>
    protected internal virtual ManualTimer CreateManualTimer(TimerCallback callback, object? state, ManualTimeProvider timeProvider)
        => new ManualTimer(callback, state, timeProvider);

    /// <summary>
    /// Sets the local time zone.
    /// </summary>
    /// <param name="localTimeZone">The local time zone.</param>
    public void SetLocalTimeZone(TimeZoneInfo localTimeZone)
    {
        this.localTimeZone = localTimeZone;
    }

    /// <summary>
    /// Advances time by a specific amount.
    /// </summary>
    /// <param name="delta">The amount of time to advance the clock by.</param>
    /// <remarks>
    /// Advancing time affects the timers created from this provider, and all other operations that are directly or
    /// indirectly using this provider as a time source. Whereas when using <see cref="TimeProvider.System"/>, time
    /// marches forward automatically in hardware, for the manual time provider the application is responsible for
    /// doing this explicitly by calling this method.
    /// <para>
    /// If advancing time moves it paste multiple scheduled timer callbacks, the current
    /// date/time reported by <see cref="GetUtcNow"/> at the point when each callback is invoked will match the
    /// due time of the callback.
    /// </para>
    /// <para>
    /// For example:
    /// <code>
    /// var start = sut.GetTimestamp();
    /// 
    /// var timer = manualTimeProvider.CreateTimer(
    ///                 callback: _ => manualTimeProvider.GetElapsedTime(start),
    ///                 state: null,
    ///                 dueTime: Span.FromSecond(1),
    ///                 period: TimeSpan.FromSecond(1));
    /// 
    /// manualtTimeProvider.Advance(TimeSpan.FromSecond(3));
    /// </code>
    /// The call to <c>Advance(TimeSpan.FromSecond(3))</c> causes the <c>timer</c>s callback to be invoked three times,
    /// and the result of the <c>manualTimeProvider.GetElapsedTime(start)</c> in the callback call will be <em>1 second</em>, <em>2 seconds</em>,
    /// and <em>3 seconds</em>. In other words, the time of the provider is set before the time callback is invoked
    /// to the time that the callback is scheduled to be invoked at.
    /// </para>
    /// <para>
    /// If the desired result is to jump time by <paramref name="delta"/> and then invoke the timer callbacks
    /// the expected number of times, i.e. such that the result of <c>manualTimeProvider.GetElapsedTime(start)</c> in the callback is
    /// <em>3 seconds</em>, <em>3 seconds</em>, and <em>3 seconds</em>, use <see cref="Jump(DateTimeOffset)"/> or <see cref="Jump(TimeSpan)"/> instead.
    /// </para>
    /// <para>
    /// Learn more about this behavior at <seealso href="https://github.com/egil/TimeProviderExtensions/#difference-between-manualtimeprovider-and-faketimeprovider"/>.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="delta"/> is negative. Going back in time is not supported.</exception>
    public void Advance(TimeSpan delta)
    {
        if (delta < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(delta), "Going back in time is not supported. ");
        }

        if (delta == TimeSpan.Zero)
        {
            return;
        }

        SetUtcNow(utcNow + delta);
    }

    /// <summary>
    /// Sets the date and time returned by <see cref="GetUtcNow()"/> to <paramref name="value"/> and triggers any
    /// scheduled items that are waiting for time to be forwarded.
    /// </summary>
    /// <param name="value">The new UtcNow time.</param>
    /// <remarks>
    /// Setting time affects the timers created from this provider, and all other operations that are directly or
    /// indirectly using this provider as a time source. Whereas when using <see cref="TimeProvider.System"/>, time
    /// marches forward automatically in hardware, for the manual time provider the application is responsible for
    /// doing this explicitly by calling this method.
    /// <para>
    /// If the set time moves it paste multiple scheduled timer callbacks, the current
    /// date/time reported by <see cref="GetUtcNow"/> at the point when each callback is invoked will match the
    /// due time of the callback.
    /// </para>
    /// <para>
    /// For example:
    /// <code>
    /// var start = sut.GetTimestamp();
    /// 
    /// var timer = manualTimeProvider.CreateTimer(
    ///                 callback: _ => manualTimeProvider.GetElapsedTime(start),
    ///                 state: null,
    ///                 dueTime: Span.FromSecond(1),
    ///                 period: TimeSpan.FromSecond(1));
    /// 
    /// manualtTimeProvider.SetUtcNow(manualtTimeProvider.Start + TimeSpan.FromSecond(3));
    /// </code>
    /// The call to <c>SetUtcNow(manualtTimeProvider.Start + TimeSpan.FromSecond(3))</c> causes the <c>timer</c>s callback to be invoked three times,
    /// and the result of the <c>manualTimeProvider.GetElapsedTime(start)</c> in the callback call will be <em>1 second</em>, <em>2 seconds</em>,
    /// and <em>3 seconds</em>. In other words, the time of the provider is set before the time callback is invoked
    /// to the time that the callback is scheduled to be invoked at.
    /// </para>
    /// <para>
    /// If the desired result is to jump to the time specified in <paramref name="value"/> and then invoke the timer callbacks
    /// the expected number of times, i.e. such that the result of <c>manualTimeProvider.GetElapsedTime(start)</c> in the callback is
    /// <em>3 seconds</em>, <em>3 seconds</em>, and <em>3 seconds</em>, use <see cref="Jump(DateTimeOffset)"/> or <see cref="Jump(TimeSpan)"/> instead.
    /// </para>
    /// <para>
    /// Learn more about this behavior at <seealso href="https://github.com/egil/TimeProviderExtensions/#difference-between-manualtimeprovider-and-faketimeprovider"/>.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less than the value returned by <see cref="GetUtcNow()"/>. Going back in time is not supported.</exception>
    public void SetUtcNow(DateTimeOffset value)
    {
        if (value < utcNow)
        {
            throw new ArgumentOutOfRangeException(nameof(value), $"The new UtcNow must be greater than or equal to the curren time {ToString()}. Going back in time is not supported.");
        }

        lock (callbacks)
        {
            // Double check in case another thread already advanced time.
            if (value <= utcNow)
            {
                return;
            }

            while (utcNow <= value && TryGetNext(value) is ManualTimerScheduler { CallbackTime: not null } scheduler)
            {
                utcNow = scheduler.CallbackTime.Value;
                scheduler.TimerElapsed();
            }

            utcNow = value;
        }

        ManualTimerScheduler? TryGetNext(DateTimeOffset targetUtcNow)
        {
            if (callbacks.Count > 0 && callbacks[0].CallbackTime <= targetUtcNow)
            {
                var callback = callbacks[0];
                callbacks.RemoveAt(0);
                return callback;
            }

            return null;
        }
    }

    /// <summary>
    /// Jumps time by a specific amount.
    /// </summary>
    /// <param name="delta">The amount of time to jump the clock by.</param>
    /// <remarks>
    /// Jumping time affects the timers created from this provider, and all other operations that are directly or
    /// indirectly using this provider as a time source. Whereas when using <see cref="TimeProvider.System"/>, time
    /// marches forward automatically in hardware, for the manual time provider the application is responsible for
    /// doing this explicitly by calling this method.
    /// <para>
    /// If jumping time moves it paste one or more scheduled timer callbacks, the current
    /// date/time reported by <see cref="GetUtcNow"/> and <see cref="GetTimestamp"/> will match the new date/time
    /// based on the <paramref name="delta"/> specified in the request.
    /// </para>
    /// <para>
    /// For example:
    /// <code>
    /// var start = sut.GetTimestamp();
    /// 
    /// var timer = manualTimeProvider.CreateTimer(
    ///                 callback: _ => manualTimeProvider.GetElapsedTime(start),
    ///                 state: null,
    ///                 dueTime: Span.FromSecond(1),
    ///                 period: TimeSpan.FromSecond(1));
    /// 
    /// manualtTimeProvider.Jump(TimeSpan.FromSecond(3));
    /// </code>
    /// The call to <c>Jump(TimeSpan.FromSecond(3))</c> causes the <c>timer</c>s callback to be invoked three times,
    /// and the result of the <c>manualTimeProvider.GetElapsedTime(start)</c> in the callback call will be <em>3 seconds</em>
    /// during all three invocations.
    /// </para>
    /// <para>
    /// If the desired result is that timer callbacks happens exactly at their scheduled callback time, i.e. such that the result
    /// of <c>manualTimeProvider.GetElapsedTime(start)</c> in the callback will be <em>1 second</em>, <em>2 seconds</em>, and <em>3 seconds</em>,
    /// use <see cref="Advance(TimeSpan)"/> or <see cref="SetUtcNow(DateTimeOffset)"/> instead.
    /// </para>
    /// <para>
    /// Learn more about this behavior at <seealso href="https://github.com/egil/TimeProviderExtensions/#difference-between-manualtimeprovider-and-faketimeprovider"/>.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="delta"/> is negative. Going back in time is not supported.</exception>
    public void Jump(TimeSpan delta)
    {
        if (delta < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(delta), "Going back in time is not supported. ");
        }

        if (delta == TimeSpan.Zero)
        {
            return;
        }

        Jump(utcNow + delta);
    }

    /// <summary>
    /// Jumps the date and time returned by <see cref="GetUtcNow()"/> to <paramref name="value"/> and triggers any
    /// scheduled items that are waiting for time to be forwarded.
    /// </summary>
    /// <param name="value">The new UtcNow time.</param>
    /// <remarks>
    /// Jumping time affects the timers created from this provider, and all other operations that are directly or
    /// indirectly using this provider as a time source. Whereas when using <see cref="TimeProvider.System"/>, time
    /// marches forward automatically in hardware, for the manual time provider the application is responsible for
    /// doing this explicitly by calling this method.
    /// <para>
    /// If jumping time moves it paste one or more scheduled timer callbacks, the current
    /// date/time reported by <see cref="GetUtcNow"/> and <see cref="GetTimestamp"/> will match the new date/time
    /// based on the <paramref name="value"/> specified in the request.
    /// </para>
    /// <para>
    /// For example:
    /// <code>
    /// var start = sut.GetTimestamp();
    /// 
    /// var timer = manualTimeProvider.CreateTimer(
    ///                 callback: _ => manualTimeProvider.GetElapsedTime(start),
    ///                 state: null,
    ///                 dueTime: Span.FromSecond(1),
    ///                 period: TimeSpan.FromSecond(1));
    ///
    /// manualtTimeProvider.Jump(manualtTimeProvider.Start + TimeSpan.FromSecond(3));
    /// </code>
    /// The call to <c>Jump(manualtTimeProvider.Start + TimeSpan.FromSecond(3))</c> causes the <c>timer</c>s callback to be invoked three times,
    /// and the result of the <c>manualTimeProvider.GetElapsedTime(start)</c> in the callback call will be <em>3 seconds</em>
    /// during all three invocations.
    /// </para>
    /// <para>
    /// If the desired result is that timer callbacks happens exactly at their scheduled callback time, i.e. such that the result
    /// of <c>manualTimeProvider.GetElapsedTime(start)</c> in the callback will be <em>1 second</em>, <em>2 seconds</em>, and <em>3 seconds</em>,
    /// use <see cref="Advance(TimeSpan)"/> or <see cref="SetUtcNow(DateTimeOffset)"/> instead.
    /// </para>
    /// <para>
    /// Learn more about this behavior at <seealso href="https://github.com/egil/TimeProviderExtensions/#difference-between-manualtimeprovider-and-faketimeprovider"/>.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less than the value returned by <see cref="GetUtcNow()"/>. Going back in time is not supported.</exception>
    public void Jump(DateTimeOffset value)
    {
        if (value < utcNow)
        {
            throw new ArgumentOutOfRangeException(nameof(value), $"The new UtcNow must be greater than or equal to the curren time {ToString()}. Going back in time is not supported.");
        }

        lock (callbacks)
        {
            // Double check in case another thread already advanced time.
            if (value <= utcNow)
            {
                return;
            }

            var jump = value - utcNow;
            utcNow = value;

            foreach (var scheduler in TryGetNext(utcNow))
            {
                // Calculates how many callbacks should have happened
                // in the jump period and invokes the callback that
                // number of times. Has to happen at least one time.
                var callbacksPassed = Math.Max(1, Math.Floor((double)jump.Ticks / scheduler.Period.Ticks));
                for (int i = 0; i < callbacksPassed; i++)
                {
                    scheduler.TimerElapsed();
                }
            }
        }

        IEnumerable<ManualTimerScheduler> TryGetNext(DateTimeOffset targetUtcNow)
        {
            while (callbacks.Count > 0 && callbacks[0].CallbackTime <= targetUtcNow)
            {
                var callback = callbacks[0];
                callbacks.RemoveAt(0);
                yield return callback;
            }
        }
    }

    /// <summary>
    /// Returns a string representation this clock's current time.
    /// </summary>
    /// <returns>A string representing the clock's current time.</returns>
    public override string ToString() => utcNow.ToString("yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture);

    internal void ScheduleCallback(ManualTimerScheduler scheduler, TimeSpan waitTime)
    {
        lock (callbacks)
        {
            scheduler.CallbackTime = utcNow + waitTime;

            var insertPosition = callbacks.FindIndex(x => x.CallbackTime > scheduler.CallbackTime);

            if (insertPosition == -1)
            {
                callbacks.Add(scheduler);
            }
            else
            {
                callbacks.Insert(insertPosition, scheduler);
            }
        }
    }

    internal void RemoveCallback(ManualTimerScheduler timerCallback)
    {
        lock (callbacks)
        {
            var existingIndexOf = callbacks.FindIndex(0, x => ReferenceEquals(x, timerCallback));
            if (existingIndexOf >= 0)
            {
                callbacks.RemoveAt(existingIndexOf);
            }
        }
    }
}
