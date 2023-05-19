using TimeProviderExtensions;

namespace System.Threading;

/// <summary>
/// PeriodicTimer extensions for <see cref="TimeProvider"/>.
/// </summary>
public static class TimeProviderPeriodicTimerExtensions
{
#if NET6_0_OR_GREATER && !NET8_0_OR_GREATER
    /// <summary>
    /// Factory method that creates a periodic timer that enables waiting asynchronously for timer ticks.
    /// Use this factory method as a replacement for instantiating a <see cref="System.Threading.PeriodicTimer"/>.
    /// </summary>
    /// <remarks>
    /// This timer is intended to be used only by a single consumer at a time: only one call to <see cref="System.Threading.PeriodicTimer.WaitForNextTickAsync" />
    /// may be in flight at any given moment. <see cref="System.Threading.PeriodicTimer.Dispose()"/> may be used concurrently with an active <see cref="System.Threading.PeriodicTimer.WaitForNextTickAsync"/>
    /// to interrupt it and cause it to return false.
    /// </remarks>
    /// <returns>
    /// A new <see cref="PeriodicTimerWrapper"/>.
    /// Note, this is a wrapper around a <see cref="System.Threading.PeriodicTimer"/>,
    /// and will behave exactly the same as the original.
    /// </returns>
    public static PeriodicTimerWrapper CreatePeriodicTimer(this TimeProvider timeProvider, TimeSpan period)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);
        return timeProvider == TimeProvider.System
            ? new PeriodicTimerWrapper.PeriodicTimerOrgWrapper(new PeriodicTimer(period))
            : new PeriodicTimerWrapper.PeriodicTimerPortWrapper(new PeriodicTimerPort(period, timeProvider));
    }
#endif
#if NET8_0_OR_GREATER
    /// <summary>
    /// Factory method that creates a periodic timer that enables waiting asynchronously for timer ticks.
    /// Use this factory method as a replacement for instantiating a <see cref="System.Threading.PeriodicTimer"/>.
    /// </summary>
    /// <remarks>
    /// This timer is intended to be used only by a single consumer at a time: only one call to <see cref="System.Threading.PeriodicTimer.WaitForNextTickAsync" />
    /// may be in flight at any given moment. <see cref="System.Threading.PeriodicTimer.Dispose()"/> may be used concurrently with an active <see cref="System.Threading.PeriodicTimer.WaitForNextTickAsync"/>
    /// to interrupt it and cause it to return false.
    /// </remarks>
    /// <returns>A new <see cref="PeriodicTimer"/>.</returns>
    public static System.Threading.PeriodicTimer CreatePeriodicTimer(this TimeProvider timeProvider, TimeSpan period)
        => new PeriodicTimer(period, timeProvider);
#endif
}