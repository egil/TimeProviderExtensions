namespace TimeScheduler.Testing;

public partial class TestScheduler
{
    /// <inheritdoc/>
    public Task Delay(TimeSpan delay)
    {
        return Delay(delay, CancellationToken.None);
    }

    /// <inheritdoc/>
    public Task Delay(TimeSpan delay, CancellationToken cancellationToken)
    {
#if NET6_0_OR_GREATER
        var tcs = new TaskCompletionSource();
#else
        var tcs = new TaskCompletionSource<object?>();
#endif

        RegisterFutureAction(
            GetUtcNow() + delay,
            () =>
            {
#if NET6_0_OR_GREATER
                tcs.TrySetResult();
#else
                lock (tcs)
                {
                    if (!tcs.Task.IsCanceled)
                    {
                        tcs.SetResult(null);
                    }
                }
#endif
            },
            () =>
            {
#if NET6_0_OR_GREATER
                tcs.TrySetCanceled();
#else
                lock (tcs)
                {
                    if (!tcs.Task.IsCanceled)
                    {
                        tcs.SetCanceled();
                    }
                }
#endif
            },
            cancellationToken);

        return tcs.Task;
    }
}
