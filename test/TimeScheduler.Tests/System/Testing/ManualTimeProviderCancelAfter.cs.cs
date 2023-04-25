namespace System.Testing;

public class ManualTimeProviderCancelAfter
{
    internal const uint MaxSupportedTimeout = 0xfffffffe;

    [Theory]
    [InlineData(-2)]
    [InlineData(MaxSupportedTimeout + 1)]
    public void CancelAfter_throws_ArgumentOutOfRangeException(double timespanInMilliseconds)
    {
        using var cts = new CancellationTokenSource();
        var sut = new ManualTimeProvider();

        var throws = () => cts.CancelAfter(TimeSpan.FromMilliseconds(timespanInMilliseconds), sut);

        throws.Should().ThrowExactly<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void CancelAfter_throws_ArgumentNullException()
    {
        using var cts = new CancellationTokenSource();

        var throws = () => cts.CancelAfter(TimeSpan.Zero, default(ManualTimeProvider)!);

        throws.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void CancelAfter_does_nothing_when_cts_already_canceled()
    {
        using var cts = new CancellationTokenSource();
        var sut = new ManualTimeProvider();

        cts.Cancel();
        cts.CancelAfter(TimeSpan.Zero, sut);

        cts.IsCancellationRequested.Should().BeTrue();
    }

    [Fact]
    public void CancelAfter_does_nothing_when_delay_eq_zero()
    {
        using var cts = new CancellationTokenSource();
        var sut = new ManualTimeProvider();

        cts.CancelAfter(TimeSpan.Zero, sut);

        cts.IsCancellationRequested.Should().BeTrue();
    }

    [Fact]
    public void CancelAfter_does_nothing_when_delay_infinite()
    {
        using var cts = new CancellationTokenSource();
        var sut = new ManualTimeProvider();

        cts.CancelAfter(TimeSpan.FromMilliseconds(-1), sut);

        cts.IsCancellationRequested.Should().BeFalse();
    }

    [Fact]
    public void CancelAfter_cancels()
    {
        using var cts = new CancellationTokenSource();
        var sut = new ManualTimeProvider();
        var delay = TimeSpan.FromMilliseconds(42);

        cts.CancelAfter(delay, sut);

        sut.ForwardTime(delay);
        cts.IsCancellationRequested.Should().BeTrue();
    }

    [Fact]
    public void CancelAfter_reschedule_longer_cancel()
    {
        using var cts = new CancellationTokenSource();
        var sut = new ManualTimeProvider();
        var initialDelay = TimeSpan.FromMilliseconds(100);
        var rescheduledDelay = TimeSpan.FromMilliseconds(1000);

        cts.CancelAfter(initialDelay, sut);
        cts.CancelAfter(rescheduledDelay, sut);

        sut.ForwardTime(initialDelay);
        cts.IsCancellationRequested.Should().BeFalse();
        sut.ForwardTime(rescheduledDelay);
        cts.IsCancellationRequested.Should().BeTrue();
    }

    [Fact]
    public void CancelAfter_reschedule_shorter_cancel()
    {
        using var cts = new CancellationTokenSource();
        var sut = new ManualTimeProvider();
        var initialDelay = TimeSpan.FromMilliseconds(1000);
        var rescheduledDelay = TimeSpan.FromMilliseconds(100);

        cts.CancelAfter(initialDelay, sut);
        cts.CancelAfter(rescheduledDelay, sut);

        sut.ForwardTime(rescheduledDelay);
        cts.IsCancellationRequested.Should().BeTrue();
    }
}
