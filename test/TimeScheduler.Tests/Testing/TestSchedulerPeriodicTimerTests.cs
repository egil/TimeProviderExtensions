namespace TimeScheduler.Testing;

public class TestSchedulerPeriodicTimerTests
{    
    [Fact]
    public void PeriodicTimer_WaitForNextTickAsync_cancelled_immediately()
    {
        using var cts = new CancellationTokenSource();
        var sut = new TestScheduler();
        using var periodicTimer = sut.PeriodicTimer(TimeSpan.FromTicks(1));

        cts.Cancel();
        var task = periodicTimer.WaitForNextTickAsync(cts.Token);

        task.IsCanceled.Should().BeTrue();
    }

    [Fact]
    public async Task PeriodicTimer_WaitForNextTickAsync_complete_immediately()
    {
        var sut = new TestScheduler();
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
        var sut = new TestScheduler(startTime);
        using var periodicTimer = sut.PeriodicTimer(TimeSpan.FromTicks(1));
        var task = periodicTimer.WaitForNextTickAsync();

        sut.ForwardTime(future);

        (await task).Should().BeTrue();
    }

    [Fact]
    public async Task PeriodicTimer_WaitForNextTickAsync_completes_after_dispose()
    {
        var startTime = DateTimeOffset.UtcNow;
        var sut = new TestScheduler(startTime);
        var periodicTimer = sut.PeriodicTimer(TimeSpan.FromTicks(1));
        var task = periodicTimer.WaitForNextTickAsync();

        periodicTimer.Dispose();

        (await task).Should().BeFalse();
    }

    [Fact]
    public async Task PeriodicTimer_WaitForNextTickAsync_cancelled_with_exception()
    {
        using var cts = new CancellationTokenSource();
        var sut = new TestScheduler();
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
        var sut = new TestScheduler();
        var calledTimes = 0;
        var interval = TimeSpan.FromSeconds(1);
        var looper = WaitForNextTickInLoop(sut, () => calledTimes++, interval);

        sut.ForwardTime(interval);
        calledTimes.Should().Be(1);

        sut.ForwardTime(interval);
        calledTimes.Should().Be(2);
    }

    [Fact]
    public void PeriodicTimer_WaitForNextTickAsync_completes_multiple_single_forward()
    {
        var sut = new TestScheduler();
        var calledTimes = 0;
        var interval = TimeSpan.FromSeconds(1);
        var looper = WaitForNextTickInLoop(sut, () => calledTimes++, interval);

        sut.ForwardTime(interval * 3);
        calledTimes.Should().Be(3);
    }

    [Fact]
    public async void PeriodicTimer_WaitForNextTickAsync_exists_on_timer_Dispose()
    {
        var sut = new TestScheduler();
        var periodicTimer = sut.CreatePeriodicTimer(TimeSpan.FromSeconds(1));
        var disposeTask = WaitForNextTickToReturnFalse(periodicTimer);
        sut.ForwardTime(TimeSpan.FromSeconds(1));

        periodicTimer.Dispose();

        (await disposeTask).Should().BeFalse();

        static async Task<bool> WaitForNextTickToReturnFalse(PeriodicTimer periodicTimer)
        {
            while (await periodicTimer.WaitForNextTickAsync(CancellationToken.None))
            {
            }

            return false;
        }
    }

    [Fact]
    public void GetUtcNow_matches_time_when_WaitForNextTickAsync_is_invoked()
    {
        var sut = new TestScheduler();
        var startTime = sut.GetUtcNow();
        var callbackTimes = new List<DateTimeOffset>();
        var interval = TimeSpan.FromSeconds(5);
        var looper = WaitForNextTickInLoop(sut, () => callbackTimes.Add(sut.GetUtcNow()), interval);

        sut.ForwardTime(interval * 3);

        callbackTimes.Should().Equal(
            startTime + interval * 1,
            startTime + interval * 2,
            startTime + interval * 3);
    }

    static async Task WaitForNextTickInLoop(TimeProvider scheduler, Action callback, TimeSpan interval)
    {
        using var periodicTimer = scheduler.CreatePeriodicTimer(interval);
        while (await periodicTimer.WaitForNextTickAsync(CancellationToken.None))
        {
            callback();
        }
    }
}
