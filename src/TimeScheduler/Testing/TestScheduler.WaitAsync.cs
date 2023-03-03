using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeScheduler.Testing;

public sealed partial class TestScheduler
{
    internal readonly struct VoidTaskResult { }
    internal const uint MaxSupportedTimeout = 0xfffffffe;
    internal const uint UnsignedInfinite = unchecked((uint)-1);

    /// <inheritdoc/>
    public Task WaitAsync(Task task, TimeSpan timeout)
        => WaitAsync(task, timeout, CancellationToken.None);

    /// <inheritdoc/>
    public Task WaitAsync(Task task, TimeSpan timeout, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(task);
        ThrowIfInvalidTimeout(timeout);

        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(cancellationToken);
        }

        if (timeout == TimeSpan.Zero)
        {
            return Task.FromException(new TimeoutException());
        }

        return WaitAsyncInternal(task, timeout, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<TResult> WaitAsync<TResult>(Task<TResult> task, TimeSpan timeout)
        => WaitAsync(task, timeout, CancellationToken.None);

    /// <inheritdoc/>
    public Task<TResult> WaitAsync<TResult>(Task<TResult> task, TimeSpan timeout, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(task);
        ThrowIfInvalidTimeout(timeout);

        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled<TResult>(cancellationToken);
        }

        if (timeout == TimeSpan.Zero)
        {
            return Task.FromException<TResult>(new TimeoutException());
        }

        return WaitAsyncInternal(task, timeout, cancellationToken);
    }

    private async Task WaitAsyncInternal(Task task, TimeSpan timeout, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource();

        var timeoutAction = RegisterFutureAction(
            UtcNow + timeout,
            () => tcs.TrySetException(new TimeoutException()),
            () => tcs.TrySetCanceled(cancellationToken),
            cancellationToken);

        await Task.WhenAny(task, tcs.Task).ConfigureAwait(false);

        if (task.IsCompleted)
        {
            timeoutAction.Cancel();
            await task.ConfigureAwait(false); ;
        }
        else
        {
            await tcs.Task.ConfigureAwait(false);
        }
    }

    private async Task<TResult> WaitAsyncInternal<TResult>(Task<TResult> task, TimeSpan timeout, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<TResult>();

        var timeoutAction = RegisterFutureAction(
            UtcNow + timeout,
            () => tcs.TrySetException(new TimeoutException()),
            () => tcs.TrySetCanceled(cancellationToken),
            cancellationToken);

        await Task.WhenAny(task, tcs.Task).ConfigureAwait(false);

        if (task.IsCompleted)
        {
            timeoutAction.Cancel();
            return await task.ConfigureAwait(false); ;
        }
        else
        {
            return await tcs.Task.ConfigureAwait(false);
        }
    }

    private static void ThrowIfInvalidTimeout(TimeSpan timeout)
    {
        long totalMilliseconds = (long)timeout.TotalMilliseconds;
        if (totalMilliseconds < -1 || totalMilliseconds > MaxSupportedTimeout)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout), $"The value needs to translate in milliseconds to -1 (signifying an infinite timeout), 0, or a positive integer less than or equal to the maximum allowed timer duration ({MaxSupportedTimeout:N0}).");
        }
    }
}
