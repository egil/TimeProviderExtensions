using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TimeScheduler.Testing;

public sealed partial class TestScheduler
{
    /// <inheritdoc/>
    public void CancelAfter(CancellationTokenSource cancellationTokenSource, TimeSpan delay)
    {
        ArgumentNullException.ThrowIfNull(cancellationTokenSource);
        ThrowIfInvalidUnspportedTimespan(delay);

        if (cancellationTokenSource.IsCancellationRequested || delay.TotalMilliseconds == UnsignedInfinite)
            return;

        if (delay == TimeSpan.Zero)
        {
            CancelWithoutExceptions();
            return;
        }

        var futureAction = RegisterAttachedFutureAction(
            UtcNow + delay,
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
