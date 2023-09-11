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

        sut.Advance(dueTime);
        callbackCount.Should().Be(1);

        sut.Advance(dueTime);
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

        sut.Advance(dueTime);
        callbackCount.Should().Be(1);

        sut.Advance(period);
        callbackCount.Should().Be(2);

        sut.Advance(period);
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

        sut.Advance(TimeSpan.FromSeconds(1));

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
        sut.Advance(dueTime);
        callbackCount.Should().Be(1);

        sut.Advance(period);
        callbackCount.Should().Be(2);

        sut.Advance(period);
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
        sut.Advance(originalDueTime);
        callbackCount.Should().Be(0);

        sut.Advance(dueTime);
        callbackCount.Should().Be(1);

        sut.Advance(period);
        callbackCount.Should().Be(2);
    }

    [Fact]
    public void Timer_callback_invoked_multiple_times_single_advance()
    {
        var sut = new ManualTimeProvider();
        var callbackCount = 0;
        var dueTime = TimeSpan.FromSeconds(3);
        var period = TimeSpan.FromSeconds(5);
        using var timer = sut.CreateTimer(_ => callbackCount++, null, dueTime, period);

        sut.Advance(TimeSpan.FromSeconds(13));

        callbackCount.Should().Be(3);
    }

    [Fact]
    public void Advancing_GetUtcNow_matches_time_at_callback_time()
    {
        var sut = new ManualTimeProvider();
        var startTime = sut.GetUtcNow();
        var callbackTimes = new List<DateTimeOffset>();
        var interval = TimeSpan.FromSeconds(3);
        using var timer = sut.CreateTimer(_ => callbackTimes.Add(sut.GetUtcNow()), null, interval, interval);

        sut.Advance(interval + interval + interval);

        callbackTimes.Should().ContainInOrder(
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

        sut.Advance(interval);
    }

    [Fact]
    public void Advancing_causes_multiple_timers_invokes_callback_in_order()
    {
        var oneSec = TimeSpan.FromSeconds(1);
        var callbacks = new List<(int TimerNumber, TimeSpan CallbackTime)>();
        var sut = new ManualTimeProvider();
        var startTime = sut.GetTimestamp();
        using var timer1 = sut.CreateTimer(_ => callbacks.Add((1, sut.GetElapsedTime(startTime))), null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
        using var timer2 = sut.CreateTimer(_ => callbacks.Add((2, sut.GetElapsedTime(startTime))), null, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3));

        sut.Advance(TimeSpan.FromSeconds(11));

        callbacks.Should().Equal(
            (1, TimeSpan.FromSeconds(2)),
            (2, TimeSpan.FromSeconds(3)),
            (1, TimeSpan.FromSeconds(4)),
            (2, TimeSpan.FromSeconds(6)),
            (1, TimeSpan.FromSeconds(6)),
            (1, TimeSpan.FromSeconds(8)),
            (2, TimeSpan.FromSeconds(9)),
            (1, TimeSpan.FromSeconds(10)));
    }

    [Fact]
    public void Jumping_causes_multiple_timers_invokes_callback_in_order()
    {
        var oneSec = TimeSpan.FromSeconds(1);
        var callbacks = new List<int>();
        var sut = new ManualTimeProvider();
        var startTime = sut.GetTimestamp();
        using var timer1 = sut.CreateTimer(_ => callbacks.Add(1), null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
        using var timer2 = sut.CreateTimer(_ => callbacks.Add(2), null, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3));

        sut.Advance(TimeSpan.FromSeconds(11));

        callbacks.Should().Equal(1, 2, 1, 2, 1, 1, 2, 1);
    }

    [Fact]
    public void Jumping_GetUtcNow_matches_jump_target()
    {
        var sut = new ManualTimeProvider();
        var startTime = sut.GetUtcNow();
        var callbackTimes = new List<DateTimeOffset>();
        var interval = TimeSpan.FromSeconds(3);
        var target = interval + interval + interval;
        using var timer = sut.CreateTimer(_ => callbackTimes.Add(sut.GetUtcNow()), null, interval, interval);

        sut.Jump(target);

        callbackTimes.Should().Equal(startTime + target, startTime + target, startTime + target);
    }

    [Fact]
    public void Jumping_past_longer_than_recurrence()
    {
        var sut = new ManualTimeProvider();
        var startTime = sut.GetUtcNow();
        var callbackTimes = new List<DateTimeOffset>();
        var interval = TimeSpan.FromSeconds(3);
        var target = TimeSpan.FromSeconds(4);
        using var timer = sut.CreateTimer(_ => callbackTimes.Add(sut.GetUtcNow()), null, interval, interval);

        sut.Jump(target);

        callbackTimes.Should().Equal(startTime + target);
    }
}