namespace TimeProviderExtensions;

public class ManualTimeProviderTimerTests
{
    [Fact]
    public void CreateTimer_with_positive_DueTime_and_infinite_Period()
    {
        var callbackCount = 0;
        var dueTime = TimeSpan.FromSeconds(1);
        var period = Timeout.InfiniteTimeSpan;
        var sut = new ManualTimeProvider();
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
        var sut = new ManualTimeProvider();
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
        var sut = new ManualTimeProvider();
        using var timer = sut.CreateTimer(_ => callbackCount++, null, dueTime, period);

        sut.ForwardTime(TimeSpan.FromSeconds(1));

        callbackCount.Should().Be(0);
    }

    [Fact]
    public void Change_timer_from_stopped_to_started()
    {
        // Arrange
        var callbackCount = 0;
        var sut = new ManualTimeProvider();
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
        var sut = new ManualTimeProvider();
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

    [Fact]
    public void Timer_callback_invoked_multiple_times_single_forward()
    {
        var callbackCount = 0;
        var sut = new ManualTimeProvider();
        var dueTime = TimeSpan.FromSeconds(3);
        var period = TimeSpan.FromSeconds(5);
        using var timer = sut.CreateTimer(_ => callbackCount++, null, dueTime, period);

        sut.ForwardTime(TimeSpan.FromSeconds(13));

        callbackCount.Should().Be(3);
    }

    [Fact]
    public void GetUtcNow_matches_time_at_callback_time()
    {
        var sut = new ManualTimeProvider();
        var startTime = sut.GetUtcNow();
        var callbackTimes = new List<DateTimeOffset>();
        var interval = TimeSpan.FromSeconds(3);
        using var timer = sut.CreateTimer(_ => callbackTimes.Add(sut.GetUtcNow()), null, interval, interval);

        sut.ForwardTime(interval + interval + interval);

        callbackTimes.Should().Equal(
            startTime + interval,
            startTime + interval + interval,
            startTime + interval + interval + interval);
    }

    [Fact]
    public void Disposing_timer_in_callback()
    {
        var interval = TimeSpan.FromSeconds(3);
        var sut = new ManualTimeProvider();
        ITimer timer = default!;
        timer = sut.CreateTimer(_ => timer!.Dispose(), null, interval, interval);

        sut.ForwardTime(interval);
    }

    [Fact]
    public void Multiple_timers_invokes_callback_in_order()
    {
        var callbacks = new List<(int TimerNumber, TimeSpan CallbackTime)>();
        var sut = new ManualTimeProvider();
        var startTime = sut.GetTimestamp();
        using var timer1 = sut.CreateTimer(_ => callbacks.Add((1, sut.GetElapsedTime(startTime))), null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
        using var timer2 = sut.CreateTimer(_ => callbacks.Add((2, sut.GetElapsedTime(startTime))), null, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3));

        sut.ForwardTime(TimeSpan.FromSeconds(10));

        var oneSec = TimeSpan.FromSeconds(1);
        callbacks.Should().Equal(
            (1, oneSec * 2),
            (2, oneSec * 3),
            (1, oneSec * 4),
            (2, oneSec * 6),
            (1, oneSec * 6),
            (1, oneSec * 8),
            (2, oneSec * 9),
            (1, oneSec * 10));
    }
}
