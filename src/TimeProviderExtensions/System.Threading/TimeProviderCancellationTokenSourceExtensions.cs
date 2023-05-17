using System.Runtime.CompilerServices;
using System.Timers;

namespace System.Threading;

/// <summary>
/// <see cref="CancellationTokenSource"/> extensions for <see cref="TimeProvider"/>.
/// </summary>
public static class TimeProviderCancellationTokenSourceExtensions
{
    private const uint MaxSupportedTimeout = 0xfffffffe;
    private const uint UnsignedInfinite = unchecked((uint)-1);
    private static readonly ConditionalWeakTable<CancellationTokenSource, DelayedCancellation> CancellationTokenSourceTimerMap = new();
    
    /// <summary>
    /// Schedules a Cancel operation on the <paramref name="cancellationTokenSource"/>.
    /// </summary>
    /// <param name="cancellationTokenSource">
    /// The <see cref="CancellationTokenSource"/> to cancel after the specified delay.
    /// </param>
    /// <param name="delay">The time span to wait before canceling the <paramref name="cancellationTokenSource"/>.
    /// </param>
    /// <param name="timeProvider">
    /// The <see cref="TimeProvider"/> to use for scheduling the cancellation.
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
    /// the <paramref name="cancellationTokenSource"/> is cancel    ed, if it has
    /// not been canceled already.
    /// </para>
    /// <para>
    /// Subsequent calls to CancelAfter will reset the delay for the
    /// <paramref name="cancellationTokenSource"/>, if it has not been canceled already.
    /// </para>
    /// </remarks>
    public static void CancelAfter(this CancellationTokenSource cancellationTokenSource, TimeSpan delay, TimeProvider timeProvider)
    {
        if (cancellationTokenSource is null)
        {
            throw new ArgumentNullException(nameof(cancellationTokenSource));
        }

        if (timeProvider is null)
        {
            throw new ArgumentNullException(nameof(timeProvider));
        }

        if (timeProvider == TimeProvider.System || delay == TimeSpan.Zero)
        {
            cancellationTokenSource.CancelAfter(delay);
            return;
        }

        ThrowIfInvalidUnspportedTimespan(delay);

        if (cancellationTokenSource.IsCancellationRequested || delay.TotalMilliseconds == UnsignedInfinite)
        {
            return;
        }

        lock (CancellationTokenSourceTimerMap)
        {
            if (CancellationTokenSourceTimerMap.TryGetValue(cancellationTokenSource, out var delayedCancellation))
            {
                delayedCancellation.SetTimeout(delay);
                return;
            }
            else
            {
                delayedCancellation = new DelayedCancellation(
                    cancellationTokenSource,
                    timeProvider,
                    () => CancellationTokenSourceTimerMap.Remove(cancellationTokenSource));
                CancellationTokenSourceTimerMap.Add(
                    cancellationTokenSource,
                    delayedCancellation);

                delayedCancellation.SetTimeout(delay);
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

    private sealed class DelayedCancellation
    {
        private readonly Action onCleanup;
        private ITimer timer;
        private CancellationTokenRegistration registration;
        private CancellationTokenSource cancellationTokenSource;

        public DelayedCancellation(
            CancellationTokenSource cancellationTokenSource,
            TimeProvider timeProvider,
            Action onCleanup)
        {
            this.cancellationTokenSource = cancellationTokenSource;
            this.onCleanup = onCleanup;
            timer = timeProvider.CreateTimer(
                _ => CancelWithoutExceptions(),
                null,
                Timeout.InfiniteTimeSpan,
                Timeout.InfiniteTimeSpan);
            registration = cancellationTokenSource.Token.Register(Cleanup);
        }

        public void SetTimeout(TimeSpan delay)
            => timer.Change(delay, Timeout.InfiniteTimeSpan);

        private void CancelWithoutExceptions()
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

        private void Cleanup()
        {
#if NET6_0_OR_GREATER
            registration.Unregister();
#endif
            timer.Dispose();
            timer = null!;
            cancellationTokenSource = null!;
            onCleanup();
        }
    }
}
