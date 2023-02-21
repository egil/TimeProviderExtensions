namespace TimeScheduler;

public class DefaultSchedulerTests
{
    [Fact]
    public void UtcNow_returns()
    {
        var sut = new DefaultScheduler();

        sut.UtcNow.Should().BeCloseTo(
            nearbyTime: DateTimeOffset.UtcNow,
            precision: TimeSpan.FromMilliseconds(50));
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
            precision: TimeSpan.FromMilliseconds(50));
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
        using var cts = new CancellationTokenSource();
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

    [Fact]
    public async void WaitAsync_with_timeout()
    {
        var sut = new DefaultScheduler();
        var task = Task.Delay(TimeSpan.FromMilliseconds(10));

        var result = sut.WaitAsync(task, TimeSpan.FromSeconds(1));

        await result
            .Should()
            .CompletedSuccessfullyAsync();
    }

    [Fact]
    public async void WaitAsync_of_T_with_timeout()
    {
        var sut = new DefaultScheduler();
        var task = StringTask();

        var result = sut.WaitAsync(task, TimeSpan.FromSeconds(1));

        await result
            .Should()
            .CompletedSuccessfullyAsync();

        async Task<string> StringTask()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(10));
            return string.Empty;
        }
    }

    [Fact]
    public async void WaitAsync_with_timeout_and_cancellationToken()
    {
        using var cts = new CancellationTokenSource();
        var sut = new DefaultScheduler();
        var task = Task.Delay(TimeSpan.FromMilliseconds(10));

        var result = sut.WaitAsync(task, TimeSpan.FromSeconds(1), cts.Token);

        await result
            .Should()
            .CompletedSuccessfullyAsync();
    }

    [Fact]
    public async void WaitAsync_of_T_with_timeout_and_cancellationToken()
    {
        using var cts = new CancellationTokenSource();
        var sut = new DefaultScheduler();
        var task = StringTask();

        var result = sut.WaitAsync(task, TimeSpan.FromSeconds(1), cts.Token);

        await result
            .Should()
            .CompletedSuccessfullyAsync();

        async Task<string> StringTask()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(10));
            return string.Empty;
        }
    }

    [Fact]
    public async void WaitAsync_with_timeout_throws()
    {
        var sut = new DefaultScheduler();
        var task = Task.Delay(TimeSpan.FromSeconds(1));

        var result = sut.WaitAsync(task, TimeSpan.FromMilliseconds(5));

        await result
            .Awaiting(x => x)
            .Should()
            .ThrowExactlyAsync<TimeoutException>();
    }

    [Fact]
    public async void WaitAsync_of_T_with_timeout_throws()
    {
        var sut = new DefaultScheduler();
        var task = StringTask();

        var result = sut.WaitAsync(task, TimeSpan.FromMilliseconds(5));

        await result
            .Awaiting(x => x)
            .Should()
            .ThrowExactlyAsync<TimeoutException>();

        async Task<string> StringTask()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            return string.Empty;
        }
    }

    [Fact]
    public async void WaitAsync_with_timeout_and_cancellationToken_throws()
    {
        using var cts = new CancellationTokenSource();
        var sut = new DefaultScheduler();
        var task = Task.Delay(TimeSpan.FromSeconds(1));

        var result = sut.WaitAsync(task, TimeSpan.FromSeconds(2), cts.Token);
        cts.Cancel();

        await result
            .Awaiting(x => x)
            .Should()
            .ThrowExactlyAsync<TaskCanceledException>();
    }

    [Fact]
    public async void WaitAsync_of_T_with_timeout_and_cancellationToken_throws()
    {
        using var cts = new CancellationTokenSource();
        var sut = new DefaultScheduler();
        var task = StringTask();

        var result = sut.WaitAsync(task, TimeSpan.FromSeconds(2), cts.Token);
        cts.Cancel();

        await result
            .Awaiting(x => x)
            .Should()
            .ThrowExactlyAsync<TaskCanceledException>();

        async Task<string> StringTask()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            return string.Empty;
        }
    }
}
