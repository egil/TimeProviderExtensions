using System.Runtime.CompilerServices;

namespace TimeProviderExtensions;

/// <summary>
/// The <see cref="AutoAdvanceBehavior"/> type provides a way to enable and customize the automatic advance of time.
/// </summary>
public sealed record class AutoAdvanceBehavior
{
    private TimeSpan clockAdvanceAmount = TimeSpan.Zero;
    private TimeSpan timestampAdvanceAmount = TimeSpan.Zero;

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

    private static void ThrowIfLessThanZero(TimeSpan value, [CallerMemberName] string? parameterName = null)
    {
        if (value < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(parameterName, "Auto advance amounts cannot be less than zero.");
        }
    }
}
