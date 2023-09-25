using System.Runtime.CompilerServices;

namespace TimeProviderExtensions;

/// <summary>
/// The <see cref="AutoAdvanceBehavior"/> type provides a way to enable and customize the automatic advance of time.
/// </summary>
public sealed record class AutoAdvanceBehavior
{
    private TimeSpan clockAdvanceAmount = TimeSpan.Zero;
    private TimeSpan timestampAdvanceAmount = TimeSpan.Zero;
    private int timerAutoInvokeCount;

    /// <summary>
    /// Gets or sets the amount of time by which time advances whenever the clock is read via <see cref="TimeProvider.GetUtcNow"/>
    /// or <see cref="TimeProvider.GetLocalNow"/>.
    /// </summary>
    /// <remarks>
    /// Set to <see cref="TimeSpan.Zero"/> to disable auto advance. The default value is <see cref="TimeSpan.Zero"/>.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when set to a value less than <see cref="TimeSpan.Zero"/>.</exception>
    public TimeSpan UtcNowAdvanceAmount { get => clockAdvanceAmount; set { ThrowIfLessThanZero(value); clockAdvanceAmount = value; } }

    /// <summary>
    /// Gets or sets the amount of time by which time advances whenever the a timestamp is read via <see cref="TimeProvider.GetTimestamp"/>
    /// or an elapsed time is calculated with <see cref="TimeProvider.GetElapsedTime(long)"/>.
    /// </summary>
    /// <remarks>
    /// Set to <see cref="TimeSpan.Zero"/> to disable auto advance. The default value is <see cref="TimeSpan.Zero"/>.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when set to a value less than <see cref="TimeSpan.Zero"/>.</exception>
    public TimeSpan TimestampAdvanceAmount { get => timestampAdvanceAmount; set { ThrowIfLessThanZero(value); timestampAdvanceAmount = value; } }

    /// <summary>
    /// <para>
    /// Gets or sets the amount of times timer callbacks will automatically be triggered.
    /// </para>
    /// <para>
    /// Setting this to a number greater than <c>0</c> causes any active timers to have their callback invoked until they have been invoked the number of times
    /// specified by <see cref="TimerAutoTriggerCount"/>. Before timer callbacks are invoked, time is advanced to match
    /// the time the callback was scheduled to be invoked, just as it is if <see cref="ManualTimeProvider.Advance(TimeSpan)"/>
    /// or <see cref="ManualTimeProvider.SetUtcNow(DateTimeOffset)"/> was manually called.
    /// </para>
    /// <para>
    /// Setting this to <c>1</c> can be used to ensure all timers, e.g. those used by <c>Task.Delay(TimeSpan, TimeProvider)</c>,
    /// <c>Task.WaitAsync(TimeSpan, TimeProvider)</c>, <c>CancellationTokenSource.CancelAfter(TimeSpan)</c> and others
    /// are completed immediately. 
    /// </para>
    /// <para>
    /// Setting this to a number larger than <c>1</c>, e.g. <c>10</c>, can be used to automatically cause a <c>PeriodicTimer(TimeSpan, TimeProvider)</c>
    /// to automatically have its <c>PeriodicTimer.WaitForNextTickAsync(CancellationToken)</c> async enumerable return <c>10</c> times.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Set to <c>0</c> to disable auto timer callback invocation. The default value is zero <c>0</c>.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when set to a value less than zero <c>0</c>.</exception>
    public int TimerAutoTriggerCount { get => timerAutoInvokeCount; set { ThrowIfLessThanZero(value); timerAutoInvokeCount = value; } }

    private static void ThrowIfLessThanZero(TimeSpan value, [CallerMemberName] string? parameterName = null)
    {
        if (value < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(parameterName, "Auto advance amounts cannot be less than zero.");
        }
    }

    private static void ThrowIfLessThanZero(int value, [CallerMemberName] string? parameterName = null)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, "Auto advance amounts cannot be less than zero.");
        }
    }
}
