namespace TimeScheduler.Testing;

public class TestSchedulerTests
{
    [Fact]
    public void ForwardTime_updates_UtcNow()
    {
        var startTime = DateTimeOffset.UtcNow;
        var sut = new TestScheduler(startTime);

        sut.ForwardTime(TimeSpan.FromTicks(1));

        sut.GetUtcNow().Should().Be(startTime + TimeSpan.FromTicks(1));
    }
}
