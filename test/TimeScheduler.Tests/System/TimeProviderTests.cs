namespace System;

public class TimeProviderTests
{
    internal const uint MaxSupportedTimeout = 0xfffffffe;

    [Fact]
    public void GetUtcNow_returns()
    {
        var sut = TimeProvider.System;

        sut.GetUtcNow().Should().BeCloseTo(
            nearbyTime: DateTimeOffset.UtcNow,
            precision: TimeSpan.FromMilliseconds(50));
    }

    [Fact]
    public async Task Delay_waits_for_n_seconds()
    {
        var sut = TimeProvider.System;
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
        var sut = TimeProvider.System;
        using var periodTimer = sut.CreatePeriodicTimer(TimeSpan.FromMilliseconds(5));

        var result = await periodTimer.WaitForNextTickAsync(CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task PeriodicTimer_cancelled_throws()
    {
        using var cts = new CancellationTokenSource();
        var sut = TimeProvider.System;
        var timerTask = TimerTask(cts.Token);

        cts.Cancel();
        cts.Dispose();

        await timerTask.Awaiting(x => x)
            .Should()
            .ThrowAsync<OperationCanceledException>();

        async Task TimerTask(CancellationToken cancellationToken)
        {
            using var periodTimer = sut.CreatePeriodicTimer(TimeSpan.FromSeconds(1));
            await periodTimer.WaitForNextTickAsync(cancellationToken);
        }
    }

    [Fact]
    public async Task WaitAsync_with_timeout()
    {
        var sut = TimeProvider.System;
        var task = Task.Delay(TimeSpan.FromMilliseconds(10));

        var result = task.WaitAsync(TimeSpan.FromSeconds(1), sut);

        await result
            .Should()
            .CompletedSuccessfullyAsync();
    }

    [Fact]
    public async Task WaitAsync_of_T_with_timeout()
    {
        var sut = TimeProvider.System;
        var task = StringTask();

        var result = task.WaitAsync(TimeSpan.FromSeconds(1), sut);

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
    public async Task WaitAsync_with_timeout_and_cancellationToken()
    {
        using var cts = new CancellationTokenSource();
        var sut = TimeProvider.System;
        var task = Task.Delay(TimeSpan.FromMilliseconds(10));

        var result = task.WaitAsync(TimeSpan.FromSeconds(1), sut, cts.Token);

        await result
            .Should()
            .CompletedSuccessfullyAsync();
    }

    [Fact]
    public async Task WaitAsync_of_T_with_timeout_and_cancellationToken()
    {
        using var cts = new CancellationTokenSource();
        var sut = TimeProvider.System;
        var task = StringTask();

        var result = task.WaitAsync(TimeSpan.FromSeconds(1), sut, cts.Token);

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
    public async Task WaitAsync_with_timeout_throws()
    {
        var sut = TimeProvider.System;
        var task = Task.Delay(TimeSpan.FromSeconds(1));

        var result = task.WaitAsync(TimeSpan.FromMilliseconds(5), sut);

        await result
            .Awaiting(x => x)
            .Should()
            .ThrowExactlyAsync<TimeoutException>();
    }

    [Fact]
    public async Task WaitAsync_of_T_with_timeout_throws()
    {
        var sut = TimeProvider.System;
        var task = StringTask();

        var result = task.WaitAsync(TimeSpan.FromMilliseconds(5), sut);

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
    public async Task WaitAsync_with_timeout_and_cancellationToken_throws()
    {
        using var cts = new CancellationTokenSource();
        var sut = TimeProvider.System;
        var task = Task.Delay(TimeSpan.FromSeconds(1));

        var result = task.WaitAsync(TimeSpan.FromSeconds(2), sut, cts.Token);
        cts.Cancel();

        await result
            .Awaiting(x => x)
            .Should()
            .ThrowExactlyAsync<TaskCanceledException>();
    }

    [Fact]
    public async Task WaitAsync_of_T_with_timeout_and_cancellationToken_throws()
    {
        using var cts = new CancellationTokenSource();
        var sut = TimeProvider.System;
        var task = StringTask();

        var result = task.WaitAsync(TimeSpan.FromSeconds(2), sut, cts.Token);
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

    [Fact]
    public void CancelAfter_throws_ObjectDisposedException()
    {
        var cts = new CancellationTokenSource();
        cts.Dispose();
        var sut = TimeProvider.System;

        var throws = () => cts.CancelAfter(TimeSpan.Zero, sut);

        throws.Should().ThrowExactly<ObjectDisposedException>();
    }

    [Theory]
    [InlineData(-2)]
    [InlineData(MaxSupportedTimeout + 1)]
    public void CancelAfter_throws_ArgumentOutOfRangeException(double timespanInMilliseconds)
    {
        using var cts = new CancellationTokenSource();
        var sut = TimeProvider.System;

        var throws = () => cts.CancelAfter(TimeSpan.FromMilliseconds(timespanInMilliseconds), sut);

        throws.Should().ThrowExactly<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void CancelAfter_throws_ArgumentNullException()
    {
        using var cts = new CancellationTokenSource();

        var throws = () => cts.CancelAfter(TimeSpan.Zero, default(TimeProvider)!);

        throws.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public async Task CancelAfter_cancels_delay_eq_zero()
    {
        using var cts = new CancellationTokenSource();
        var sut = TimeProvider.System;

        cts.CancelAfter(TimeSpan.Zero, sut);

        await sut.Delay(TimeSpan.FromMilliseconds(50));
        cts.IsCancellationRequested.Should().BeTrue();
    }

    [Fact]
    public async Task CancelAfter_cancels()
    {
        using var cts = new CancellationTokenSource();
        var sut = TimeProvider.System;
        var delay = TimeSpan.FromMilliseconds(30);

        cts.CancelAfter(delay, sut);

        var throwsAfterCancel = () => Task.Delay(TimeSpan.FromHours(1), cts.Token);
        await throwsAfterCancel.Should().ThrowAsync<TaskCanceledException>();
    }

    [Fact]
    public async Task CancelAfter_reschedule_cancel()
    {
        using var cts = new CancellationTokenSource();
        var sut = TimeProvider.System;
        var delay1 = TimeSpan.FromSeconds(1);
        var delay2 = TimeSpan.FromMilliseconds(50);

        cts.CancelAfter(delay1, sut);
        cts.CancelAfter(delay2, sut);

        // add extra buffer to ensure cancellation has been processed
        await sut.Delay(delay2 * 2);
        cts.IsCancellationRequested.Should().BeTrue();
    }
}