namespace System.Testing;

public class TestTimeProviderDelayTests
{
    [Fact]
    public void Delayed_task_is_completes()
    {
        var startTime = DateTimeOffset.UtcNow;
        var future = TimeSpan.FromTicks(1);
        using var sut = new TestTimeProvider(startTime);
        var task = sut.Delay(TimeSpan.FromTicks(1));

        sut.ForwardTime(future);

        task.Status.Should().Be(TaskStatus.RanToCompletion);
    }

    [Fact]
    public void Delayed_task_is_cancelled()
    {
        using var cts = new CancellationTokenSource();
        using var sut = new TestTimeProvider();
        var task = sut.Delay(TimeSpan.FromTicks(1), cts.Token);

        cts.Cancel();

        task.Status.Should().Be(TaskStatus.Canceled);
    }
}
