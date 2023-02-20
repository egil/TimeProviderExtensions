using System.Diagnostics;
using System.Threading;

namespace TimeScheduler;

public class DefaultSchedulerTests
{
    [Fact]
    public void UtcNow_returns()
    {
        var sut = new DefaultScheduler();

        sut.UtcNow.Should().BeCloseTo(
            nearbyTime: DateTimeOffset.UtcNow,
            precision: TimeSpan.FromMilliseconds(20));
    }

    [Fact]
    public async Task Delay_waits_for_n_seconds()
    {
        var sut = new DefaultScheduler();
        var timer = Stopwatch.StartNew();

        await sut.Delay(TimeSpan.FromSeconds(1));

        timer.Stop();
        timer.Elapsed.Should().BeCloseTo(
            nearbyTime: TimeSpan.FromSeconds(1),
            precision: TimeSpan.FromMilliseconds(20));
    }

    [Fact]
    public async Task PeriodicTimer()
    {
        var sut = new DefaultScheduler();
        using var periodTimer = sut.PeriodicTimer(TimeSpan.FromMilliseconds(5));

        var result = await periodTimer.WaitForNextTickAsync(CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task PeriodicTimer_cancelled_throws()
    {
        var cts = new CancellationTokenSource();
        var sut = new DefaultScheduler();
        var timerTask = TimerTask(cts.Token);

        cts.Cancel();
        cts.Dispose();

        await timerTask.Awaiting(x => x)
            .Should()
            .ThrowAsync<OperationCanceledException>();

        async Task TimerTask(CancellationToken cancellationToken)
        {
            using var periodTimer = sut.PeriodicTimer(TimeSpan.FromSeconds(1));
            await periodTimer.WaitForNextTickAsync(cancellationToken);
        }
    }
}
