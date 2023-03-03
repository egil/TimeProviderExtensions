using TimeScheduler.Testing;

namespace TimeScheduler;

public class TestSchedulerCancelAfter
{
    internal const uint MaxSupportedTimeout = 0xfffffffe;

    [Theory]
    [InlineData(-2)]
    [InlineData(MaxSupportedTimeout + 1)]
    public void CancelAfter_throws_ArgumentOutOfRangeException(double timespanInMilliseconds)
    {
        using var cts = new CancellationTokenSource();
        using var sut = new TestScheduler();

        var throws = () => sut.CancelAfter(cts, TimeSpan.FromMilliseconds(timespanInMilliseconds));

        throws.Should().ThrowExactly<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void CancelAfter_throws_ArgumentNullException()
    {
        using var sut = new TestScheduler();

        var throws = () => sut.CancelAfter(default!, TimeSpan.Zero);

        throws.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void CancelAfter_does_nothing_when_cts_already_canceled()
    {
        using var cts = new CancellationTokenSource();
        using var sut = new TestScheduler();

        cts.Cancel();
        sut.CancelAfter(cts, TimeSpan.Zero);

        cts.IsCancellationRequested.Should().BeTrue();
    }

    [Fact]
    public void CancelAfter_does_nothing_when_delay_eq_zero()
    {
        using var cts = new CancellationTokenSource();
        using var sut = new TestScheduler();

        sut.CancelAfter(cts, TimeSpan.Zero);

        cts.IsCancellationRequested.Should().BeTrue();
    }

    [Fact]
    public void CancelAfter_does_nothing_when_delay_infinite()
    {
        using var cts = new CancellationTokenSource();
        using var sut = new TestScheduler();

        sut.CancelAfter(cts, TimeSpan.FromMilliseconds(-1));

        cts.IsCancellationRequested.Should().BeFalse();
    }

    [Fact]
    public void CancelAfter_cancels()
    {
        using var cts = new CancellationTokenSource();
        using var sut = new TestScheduler();
        var delay = TimeSpan.FromMilliseconds(42);

        sut.CancelAfter(cts, delay);

        sut.ForwardTime(delay);
        cts.IsCancellationRequested.Should().BeTrue();
    }

    [Fact]
    public void CancelAfter_reschedule_longer_cancel()
    {
        using var cts = new CancellationTokenSource();
        using var sut = new TestScheduler();
        var initialDelay = TimeSpan.FromMilliseconds(100);
        var rescheduledDelay = TimeSpan.FromMilliseconds(1000);

        sut.CancelAfter(cts, initialDelay);
        sut.CancelAfter(cts, rescheduledDelay);

        sut.ForwardTime(initialDelay);
        cts.IsCancellationRequested.Should().BeFalse();
        sut.ForwardTime(rescheduledDelay);
        cts.IsCancellationRequested.Should().BeTrue();
    }

    [Fact]
    public void CancelAfter_reschedule_shorter_cancel()
    {
        using var cts = new CancellationTokenSource();
        using var sut = new TestScheduler();
        var initialDelay = TimeSpan.FromMilliseconds(1000);
        var rescheduledDelay = TimeSpan.FromMilliseconds(100);

        sut.CancelAfter(cts, initialDelay);
        sut.CancelAfter(cts, rescheduledDelay);

        sut.ForwardTime(rescheduledDelay);
        cts.IsCancellationRequested.Should().BeTrue();
    }
}
