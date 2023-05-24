#if TargetMicrosoftTestTimeProvider
using SutTimeProvider = Microsoft.Extensions.Time.Testing.FakeTimeProvider;
#else
using SutTimeProvider = TimeProviderExtensions.ManualTimeProvider;
#endif

namespace TimeProviderExtensions;

public class ManualTimeProviderDelayTests
{
    [Fact]
    public void Delayed_task_is_completes()
    {
        var startTime = DateTimeOffset.UtcNow;
        var future = TimeSpan.FromMilliseconds(1);
        var sut = new SutTimeProvider(startTime);
        var task = sut.Delay(TimeSpan.FromMilliseconds(1));

        sut.Advance(future);

        task.Status.Should().Be(TaskStatus.RanToCompletion);
    }

    [Fact]
    public void Delayed_task_is_cancelled()
    {
        using var cts = new CancellationTokenSource();
        var sut = new SutTimeProvider();
        var task = sut.Delay(TimeSpan.FromMilliseconds(1), cts.Token);

        cts.Cancel();

        task.Status.Should().Be(TaskStatus.Canceled);
    }
}
