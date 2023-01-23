using System.Diagnostics;

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

        var result =await periodTimer.WaitForNextTickAsync(CancellationToken.None);

        result.Should().BeTrue();
    }
}
