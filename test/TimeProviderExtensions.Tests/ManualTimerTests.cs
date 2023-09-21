using FluentAssertions.Extensions;

namespace TimeProviderExtensions;

public class ManualTimerTests
{
    [Fact]
    public void ToString_with_disabled_timer()
    {
        var timeProvider = new ManualTimeProvider();

        var sut = timeProvider.CreateTimer(_ => { }, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

        sut.ToString().Should().Be("Timer is disabled. DueTime: Infinite. Period: Infinite.");
    }

    [Fact]
    public void ToString_with_immidiate_invokcation_timer()
    {
        var timeProvider = new ManualTimeProvider();

        var sut = timeProvider.CreateTimer(_ => { }, null, TimeSpan.Zero, Timeout.InfiniteTimeSpan);

        sut.ToString().Should().Be("Timer is disabled. DueTime: 00:00:00. Period: Infinite.");
    }

    [Fact]
    public void ToString_with_periodic_timer()
    {
        var timeProvider = new ManualTimeProvider();

        var sut = timeProvider.CreateTimer(_ => { }, null, TimeSpan.Zero, new TimeSpan(hours: 23, minutes: 33, seconds: 2));

        sut.ToString().Should().Be("Next callback: 2000-01-01T23:33:02.000. DueTime: 00:00:00. Period: 23:33:02.");
    }

    [Fact]
    public void ToString_with_duetime_periodic_timer()
    {
        var timeProvider = new ManualTimeProvider();

        var sut = timeProvider.CreateTimer(_ => { }, null, 2.Seconds(), new TimeSpan(hours: 23, minutes: 33, seconds: 2));

        sut.ToString().Should().Be("Next callback: 2000-01-01T00:00:02.000. DueTime: 00:00:02. Period: 23:33:02.");
    }

    [Fact]
    public void CallbackTime_with_inactive_timer()
    {
        var timeProvider = new ManualTimeProvider();

        var sut = (ManualTimer)timeProvider.CreateTimer(_ => { }, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

        sut.CallbackTime.Should().Be(default(DateTimeOffset?));
    }

    [Fact]
    public void CallbackTime_with_active_timer()
    {
        var timeProvider = new ManualTimeProvider();

        var sut = (ManualTimer)timeProvider.CreateTimer(_ => { }, null, 2.Seconds(), Timeout.InfiniteTimeSpan);

        sut.CallbackTime.Should().Be(timeProvider.Start + 2.Seconds());
    }
}
