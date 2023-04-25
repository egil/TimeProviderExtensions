namespace TimeScheduler.Testing;

public partial class TestScheduler
{
    /// <inheritdoc/>
    public void CancelAfter(CancellationTokenSource cancellationTokenSource, TimeSpan delay)
    {
        if (cancellationTokenSource is null)
        {
            throw new ArgumentNullException(nameof(cancellationTokenSource));
        }

        ThrowIfInvalidUnspportedTimespan(delay);

        if (cancellationTokenSource.IsCancellationRequested || delay.TotalMilliseconds == UnsignedInfinite)
            return;

        if (delay == TimeSpan.Zero)
        {
            CancelWithoutExceptions();
            return;
        }

        var futureAction = RegisterAttachedFutureAction(
            GetUtcNow() + delay,
            cancellationTokenSource,
            CancelWithoutExceptions,
            static () => { },
            cancellationTokenSource.Token);

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
}
