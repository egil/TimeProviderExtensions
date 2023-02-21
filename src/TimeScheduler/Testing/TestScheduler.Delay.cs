namespace TimeScheduler.Testing;

public sealed partial class TestScheduler
{
    public Task Delay(TimeSpan delay)
    {
        return Delay(delay, CancellationToken.None);
    }

    public Task Delay(TimeSpan delay, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource();
        RegisterFutureAction(UtcNow + delay, () => tcs.TrySetResult(), () => tcs.TrySetCanceled(), cancellationToken);
        return tcs.Task;
    }
}
