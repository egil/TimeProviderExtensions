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
}