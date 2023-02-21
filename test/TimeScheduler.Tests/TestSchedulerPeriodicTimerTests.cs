namespace TimeScheduler.Testing;

public class TestSchedulerPeriodicTimerTests
{    
    [Fact]
    public void PeriodicTimer_WaitForNextTickAsync_cancelled_immediately()
    {
        using var cts = new CancellationTokenSource();
        using var sut = new TestScheduler();
        using var periodicTimer = sut.PeriodicTimer(TimeSpan.FromTicks(1));

        cts.Cancel();
        var task = periodicTimer.WaitForNextTickAsync(cts.Token);

        task.IsCanceled.Should().BeTrue();
    }

    [Fact]
    public async Task PeriodicTimer_WaitForNextTickAsync_complete_immediately()
    {
        using var sut = new TestScheduler();
        using var periodicTimer = sut.PeriodicTimer(TimeSpan.FromTicks(1));

        sut.ForwardTime(TimeSpan.FromTicks(1));
        var task = periodicTimer.WaitForNextTickAsync();

        (await task).Should().BeTrue();
    }

    [Fact]
    public async Task PeriodicTimer_WaitForNextTickAsync_completes()
    {
        var startTime = DateTimeOffset.UtcNow;
        var future = TimeSpan.FromTicks(1);
        using var sut = new TestScheduler(startTime);
        using var periodicTimer = sut.PeriodicTimer(TimeSpan.FromTicks(1));
        var task = periodicTimer.WaitForNextTickAsync();

        sut.ForwardTime(future);

        (await task).Should().BeTrue();
    }

    [Fact]
    public async Task PeriodicTimer_WaitForNextTickAsync_completes_after_dispose()
    {
        var startTime = DateTimeOffset.UtcNow;
        using var sut = new TestScheduler(startTime);
        var periodicTimer = sut.PeriodicTimer(TimeSpan.FromTicks(1));
        var task = periodicTimer.WaitForNextTickAsync();

        periodicTimer.Dispose();

        (await task).Should().BeFalse();
    }

    [Fact]
    public async Task PeriodicTimer_WaitForNextTickAsync_cancelled_with_exception()
    {
        using var cts = new CancellationTokenSource();
        using var sut = new TestScheduler();
        using var periodicTimer = sut.PeriodicTimer(TimeSpan.FromTicks(1));
        var task = periodicTimer.WaitForNextTickAsync(cts.Token);
        cts.CancelAfter(TimeSpan.Zero);

        var throws = async () => await task;

        await throws
            .Should()
            .ThrowExactlyAsync<TaskCanceledException>();
    }

    [Fact]
    public void PeriodicTimer_WaitForNextTickAsync_completes_multiple()
    {
        using var sut = new TestScheduler();
        var calledTimes = 0;
        var looper = WaitForNextTickInLoop(sut, () => calledTimes++);

        sut.ForwardTime(TimeSpan.FromSeconds(1));
        calledTimes.Should().Be(1);

        sut.ForwardTime(TimeSpan.FromSeconds(1));
        calledTimes.Should().Be(2);

        static async Task WaitForNextTickInLoop(ITimeScheduler scheduler, Action callback)
        {
            using var periodicTimer = scheduler.PeriodicTimer(TimeSpan.FromSeconds(1));
            while (await periodicTimer.WaitForNextTickAsync(CancellationToken.None))
            {
                callback();
            }
        }
    }
}
