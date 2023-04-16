namespace TimeScheduler.Testing;

public class TestSchedulerTimerTests
{
    [Fact]
    public void CreateTimer_with_positive_DueTime_and_infinite_Period()
    {
        var callbackCount = 0;
        var dueTime = TimeSpan.FromSeconds(1);
        var period = Timeout.InfiniteTimeSpan;
        using var sut = new TestScheduler();
        using var timer = sut.CreateTimer(_ => callbackCount++, null, dueTime, period);

        sut.ForwardTime(dueTime);
        callbackCount.Should().Be(1);

        sut.ForwardTime(dueTime);
        callbackCount.Should().Be(1);
    }

    [Fact]
    public void CreateTimer_with_positive_DueTime_and_Period()
    {
        var callbackCount = 0;
        var dueTime = TimeSpan.FromSeconds(1);
        var period = TimeSpan.FromSeconds(2);
        using var sut = new TestScheduler();
        using var timer = sut.CreateTimer(_ => callbackCount++, null, dueTime, period);

        sut.ForwardTime(dueTime);
        callbackCount.Should().Be(1);

        sut.ForwardTime(period);
        callbackCount.Should().Be(2);

        sut.ForwardTime(period);
        callbackCount.Should().Be(3);
    }

    [Fact]
    public void CreateTimer_with_infinite_DueTime_and_Period()
    {
        var callbackCount = 0;
        var dueTime = Timeout.InfiniteTimeSpan;
        var period = Timeout.InfiniteTimeSpan;
        using var sut = new TestScheduler();
        using var timer = sut.CreateTimer(_ => callbackCount++, null, dueTime, period);

        sut.ForwardTime(TimeSpan.FromSeconds(1));

        callbackCount.Should().Be(0);
    }

    [Fact]
    public void Change_timer_from_stopped_to_started()
    {
        // Arrange
        var callbackCount = 0;
        using var sut = new TestScheduler();
        using var timer = sut.CreateTimer(_ => callbackCount++, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        var dueTime = TimeSpan.FromSeconds(1);
        var period = TimeSpan.FromSeconds(2);

        // Act
        timer.Change(dueTime, period);

        // Assert
        sut.ForwardTime(dueTime);
        callbackCount.Should().Be(1);

        sut.ForwardTime(period);
        callbackCount.Should().Be(2);

        sut.ForwardTime(period);
        callbackCount.Should().Be(3);
    }

    [Fact]
    public void Change_timer()
    {
        // Arrange
        var callbackCount = 0;
        using var sut = new TestScheduler();
        var originalDueTime = TimeSpan.FromSeconds(3);
        var period = TimeSpan.FromSeconds(5);
        using var timer = sut.CreateTimer(_ => callbackCount++, null, originalDueTime, period);
        var dueTime = TimeSpan.FromSeconds(4);

        // Change to a larger value
        timer.Change(dueTime, period);

        // Assert that previous dueTime is ignored
        sut.ForwardTime(originalDueTime);
        callbackCount.Should().Be(0);

        sut.ForwardTime(dueTime);
        callbackCount.Should().Be(1);

        sut.ForwardTime(period);
        callbackCount.Should().Be(2);
    }
}
