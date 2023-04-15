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
        var tcs = new TaskCompletionSource();

        RegisterFutureAction(
            GetUtcNow() + delay,
            () => tcs.TrySetResult(),
            () => tcs.TrySetCanceled(),
            cancellationToken);

        return tcs.Task;
    }
}
