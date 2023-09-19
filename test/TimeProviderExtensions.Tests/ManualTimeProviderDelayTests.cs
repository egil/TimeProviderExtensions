namespace TimeProviderExtensions;

public class ManualTimeProviderDelayTests
{
    [Fact]
    public void Delayed_task_is_completes()
    {
        var startTime = DateTimeOffset.UtcNow;
        var future = TimeSpan.FromMilliseconds(1);
        var sut = new ManualTimeProvider(startTime);
        var task = sut.Delay(TimeSpan.FromMilliseconds(1));

        sut.Advance(future);

        task.Status.Should().Be(TaskStatus.RanToCompletion);
    }

#if NET8_0_OR_GREATER
    [Fact]
#else
    [Fact(Skip = "Bug in .NET 7 and earlier - https://github.com/dotnet/runtime/issues/92264")]
#endif
    public void Delayed_task_is_cancelled()
    {
        using var cts = new CancellationTokenSource();
        var sut = new ManualTimeProvider();
        var task = sut.Delay(TimeSpan.FromMilliseconds(1), cts.Token);

        cts.Cancel();

        task.Status.Should().Be(TaskStatus.Canceled);
    }
}
