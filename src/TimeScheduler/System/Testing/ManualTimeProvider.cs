using TimeScheduler.Testing;

namespace System.Testing;

/// <summary>
/// Represents a test implementation of a <see cref="TimeProvider"/>,
/// where time stands still until you call <see cref="ForwardTime(TimeSpan)"/>
/// or <see cref="SetUtcNow(DateTimeOffset)"/>.
/// </summary>
/// <remarks>
/// Learn more at <see href="https://github.com/egil/TimeScheduler"/>.
/// </remarks>
public sealed class ManualTimeProvider : TimeProvider
{
    private readonly TestScheduler testScheduler;

    /// <summary>
    /// Creates an instance of the <see cref="ManualTimeProvider"/> with
    /// <see cref="DateTimeOffset.UtcNow"/> being the initial value returned by <see cref="GetUtcNow()"/>.
    /// </summary>
    public ManualTimeProvider()
        : this(DateTimeOffset.UtcNow)
    {
    }

    /// <summary>
    /// Creates an instance of the <see cref="ManualTimeProvider"/> with
    /// <paramref name="startDateTime"/> being the initial value returned by <see cref="GetUtcNow()"/>.
    /// </summary>
    /// <param name="startDateTime">The initial date and time <see cref="GetUtcNow()"/> will return.</param>
    public ManualTimeProvider(DateTimeOffset startDateTime)
    {
        testScheduler = new(startDateTime);
    }

    /// <summary>
    /// Gets the frequency of <see cref="GetTimestamp"/> of high-frequency value per second.
    /// </summary>
    /// <remarks>
    /// This implementation bases timestamp on <see cref="DateTimeOffset.UtcTicks"/>, which is 10,000 ticks per millisecond,
    /// since the progression of time is represented by the date and time returned from <see cref="GetUtcNow()" />.
    /// </remarks>
    public override long TimestampFrequency { get; } = 10_000_000;

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
    public override DateTimeOffset GetUtcNow() => testScheduler.GetUtcNow();

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
        => testScheduler.CreateTimer(callback, state, dueTime, period);

    /// <summary>
    /// Sets the date and time returned by <see cref="GetUtcNow()"/> to <paramref name="newUtcNew"/> and triggers any
    /// scheduled items that are waiting for time to be forwarded.
    /// </summary>
    /// <param name="newUtcNew">The new UtcNow time.</param>
    /// <exception cref="ArgumentException">If <paramref name="newUtcNew"/> is less than the value returned by <see cref="GetUtcNow()"/>.</exception>
    public void SetUtcNow(DateTimeOffset newUtcNew) => testScheduler.SetUtcNow(newUtcNew);

    /// <summary>
    /// Forward the date and time represented by <see cref="GetUtcNow()"/>
    /// by the specified <paramref name="time"/>, and triggers any
    /// scheduled items that are waiting for time to be forwarded.
    /// </summary>
    /// <param name="time">The span of time to forward <see cref="GetUtcNow()"/> with.</param>
    /// <exception cref="ArgumentException">If <paramref name="time"/> is negative or zero.</exception>
    public void ForwardTime(TimeSpan time) => testScheduler.ForwardTime(time);
}
