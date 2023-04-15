using System.Runtime.CompilerServices;
using TimeScheduler;

namespace System.Threading;

/// <summary>
/// <see cref="CancellationTokenSource"/> extensions for <see cref="TimeProvider"/>.
/// </summary>
public static class TimeProviderCancellationTokenSourceExtensions
{
    private const uint MaxSupportedTimeout = 0xfffffffe;
    private const uint UnsignedInfinite = unchecked((uint)-1);
    private static readonly ConditionalWeakTable<CancellationTokenSource, ITimer> CancellationTokenSourceTimerMap = new();

    /// <summary>
    /// Schedules a Cancel operation on the <paramref name="cancellationTokenSource"/>.
    /// </summary>
    /// <param name="timeProvider">
    /// The <see cref="TimeProvider"/> to use for scheduling the cancellation.
    /// </param>
    /// <param name="cancellationTokenSource">
    /// The <see cref="CancellationTokenSource"/> to cancel after the specified delay.
    /// </param>
    /// <param name="delay">The time span to wait before canceling the <paramref name="cancellationTokenSource"/>.
    /// </param>
    /// <exception cref="ObjectDisposedException">The exception thrown when the <paramref name="cancellationTokenSource"/>
    /// has been disposed.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The <paramref name="delay"/> is less than -1 or greater than maximum allowed timer duration.
    /// </exception>
    /// <exception cref="ArgumentNullException">The <paramref name="cancellationTokenSource"/> is null.</exception>
    /// <remarks>
    /// <para>
    /// The countdown for the delay starts during this call. When the delay expires,
    /// the <paramref name="cancellationTokenSource"/> is canceled, if it has
    /// not been canceled already.
    /// </para>
    /// <para>
    /// Subsequent calls to CancelAfter will reset the delay for the
    /// <paramref name="cancellationTokenSource"/>, if it has not been canceled already.
    /// </para>
    /// </remarks>
    public static void CancelAfter(this CancellationTokenSource cancellationTokenSource, TimeSpan delay, TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(cancellationTokenSource);
        ArgumentNullException.ThrowIfNull(timeProvider);

        if (timeProvider == TimeProvider.System)
        {
            cancellationTokenSource.CancelAfter(delay);
        }

        ThrowIfInvalidUnspportedTimespan(delay);

        if (cancellationTokenSource.IsCancellationRequested || delay.TotalMilliseconds == UnsignedInfinite)
        {
            return;
        }

        if (delay == TimeSpan.Zero)
        {
            CancelWithoutExceptions();
            return;
        }

        lock (CancellationTokenSourceTimerMap)
        {
            if (CancellationTokenSourceTimerMap.TryGetValue(cancellationTokenSource, out var timer))
            {
                timer.Change(delay, Timeout.InfiniteTimeSpan);
                return;
            }

            timer = timeProvider.CreateTimer(
                _ => CancelWithoutExceptions(),
                null,
                delay,
                Timeout.InfiniteTimeSpan);

            CancellationTokenSourceTimerMap.Add(cancellationTokenSource, timer);

            var registration = cancellationTokenSource.Token.Register(
                static (state) =>
                {
                    var timer = (ITimer)state!;
                    timer.Dispose();
                },
                state: timer);
        }

        void CancelWithoutExceptions()
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            try
            {
                cancellationTokenSource.Cancel();
            }
            catch (ObjectDisposedException)
            {
            }
            catch (AggregateException)
            {
            }
        }
    }

    private static void ThrowIfInvalidUnspportedTimespan(TimeSpan timespan, [CallerArgumentExpression("timespan")] string? paramName = null)
    {
        long totalMilliseconds = (long)timespan.TotalMilliseconds;
        if (totalMilliseconds < -1 || totalMilliseconds > MaxSupportedTimeout)
        {
            throw new ArgumentOutOfRangeException(paramName, $"The value needs to translate in milliseconds to -1 (signifying an infinite timeout), 0, or a positive integer less than or equal to the maximum allowed timer duration ({MaxSupportedTimeout:N0}).");
        }
    }
}
