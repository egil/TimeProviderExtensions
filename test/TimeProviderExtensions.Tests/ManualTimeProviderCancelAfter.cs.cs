namespace TimeProviderExtensions;

public class ManualTimeProviderCancelAfter
{
    [Fact]
    public void CancelAfter_cancels()
    {
        var delay = TimeSpan.FromMilliseconds(42);
        var sut = new ManualTimeProvider();
        using var cts = sut.CreateCancellationTokenSource(delay);

        sut.ForwardTime(delay);

        cts.IsCancellationRequested.Should().BeTrue();
    }

#if NET8_0_OR_GREATER
    // The following two scenarios are only supported by .NET 8 and up.
    [Fact]
    public void CancelAfter_reschedule_longer_cancel()
    {
        var initialDelay = TimeSpan.FromMilliseconds(100);
        var rescheduledDelay = TimeSpan.FromMilliseconds(1000);
        var sut = new ManualTimeProvider();
        using var cts = sut.CreateCancellationTokenSource(initialDelay);

        cts.CancelAfter(rescheduledDelay);

        sut.ForwardTime(initialDelay);
        cts.IsCancellationRequested.Should().BeFalse();

        sut.ForwardTime(rescheduledDelay - initialDelay);
        cts.IsCancellationRequested.Should().BeTrue();
    }

    [Fact]
    public async void CancelAfter_reschedule_shorter_cancel()
    {
        var initialDelay = TimeSpan.FromMilliseconds(1000);
        var rescheduledDelay = TimeSpan.FromMilliseconds(100);
        var sut = new ManualTimeProvider();
        using var cts = sut.CreateCancellationTokenSource(initialDelay);

        cts.CancelAfter(rescheduledDelay);

        sut.ForwardTime(rescheduledDelay);
        await Task.Delay(1000);

        cts.IsCancellationRequested.Should().BeTrue();
    }
#endif
}
