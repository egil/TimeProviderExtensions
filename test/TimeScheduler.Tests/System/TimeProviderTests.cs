namespace System;

public class TimeProviderTests
{
    internal const uint MaxSupportedTimeout = 0xfffffffe;

#if NET6_0_OR_GREATER
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
#endif

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
    public async Task CreateCancellationTokenSource_cancels()
    {
        var delay = TimeSpan.FromMilliseconds(30);
        using var cts = TimeProvider.System.CreateCancellationTokenSource(delay);

        var throwsAfterCancel = () => Task.Delay(TimeSpan.FromHours(1), cts.Token);
        await throwsAfterCancel.Should().ThrowAsync<TaskCanceledException>();
    }

    [Fact]
    public async Task CancelAfter_reschedule_cancel()
    {
        using var cts = new CancellationTokenSource();
        var sut = TimeProvider.System;
        var delay1 = TimeSpan.FromSeconds(100);
        var delay2 = TimeSpan.FromMilliseconds(20);

        cts.CancelAfter(delay1, sut);
        cts.CancelAfter(delay2, sut);

        // add extra buffer to ensure cancellation has been processed
        await sut.Delay(delay2 + delay2);
        cts.IsCancellationRequested.Should().BeTrue();
    }
}