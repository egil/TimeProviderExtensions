using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TimeScheduler.Testing;

public sealed partial class TestScheduler
{
    internal readonly struct VoidTaskResult { }

    /// <inheritdoc/>
    public Task WaitAsync(Task task, TimeSpan timeout)
        => WaitAsync(task, timeout, CancellationToken.None);

    /// <inheritdoc/>
    public Task WaitAsync(Task task, TimeSpan timeout, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(task);
        ThrowIfInvalidUnspportedTimespan(timeout);

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
        ThrowIfInvalidUnspportedTimespan(timeout);

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
}
