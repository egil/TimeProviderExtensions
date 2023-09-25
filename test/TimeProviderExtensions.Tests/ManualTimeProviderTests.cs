namespace TimeProviderExtensions;

public class ManualTimeProviderTests
{
    [Fact]
    public void Advance_updates_UtcNow()
    {
        var startTime = DateTimeOffset.UtcNow;
        var sut = new ManualTimeProvider(startTime);

        sut.Advance(TimeSpan.FromTicks(1));

        sut.GetUtcNow().Should().Be(startTime + TimeSpan.FromTicks(1));
    }

    [Fact]
    public void SetUtcNow_updates_UtcNow()
    {
        var startTime = DateTimeOffset.UtcNow;
        var sut = new ManualTimeProvider(startTime);

        sut.SetUtcNow(startTime + TimeSpan.FromTicks(1));

        sut.GetUtcNow().Should().Be(startTime + TimeSpan.FromTicks(1));
    }

    [Fact]
    public async Task Delay_callbacks_runs_synchronously()
    {
        // arrange
        var sut = new ManualTimeProvider();
        var callbackCount = 0;
        var continuationTask = Continuation(sut, () => callbackCount++);

        // act
        sut.Advance(TimeSpan.FromSeconds(10));

        // assert
        callbackCount.Should().Be(1);
        await continuationTask;

        static async Task Continuation(TimeProvider timeProvider, Action callback)
        {
            await timeProvider.Delay(TimeSpan.FromSeconds(10));
            callback();
        }
    }

#if NET8_0_OR_GREATER
    [Fact]
#else
    [Fact(Skip = "Bug in .NET 7 and earlier - https://github.com/dotnet/runtime/issues/92264")]
#endif
    public async Task WaitAsync_callbacks_runs_synchronously()
    {
        // arrange
        var sut = new ManualTimeProvider();
        var callbackCount = 0;
        var continuationTask = Continuation(sut, () => callbackCount++);

        // act
        sut.Advance(TimeSpan.FromSeconds(10));

        // assert
        callbackCount.Should().Be(1);
        await continuationTask;

        static async Task Continuation(TimeProvider timeProvider, Action callback)
        {
            try
            {
                await Task
                    .Delay(TimeSpan.FromDays(1))
                    .WaitAsync(TimeSpan.FromSeconds(10), timeProvider);
            }
            catch (TimeoutException)
            {
                callback();
            }
        }
    }

#if !NET8_0_OR_GREATER && NET6_0_OR_GREATER
    [Fact]
    public async Task Callbacks_happens_in_schedule_order()
    {
        var sut = new ManualTimeProvider();
        var periodicTimer = sut.CreatePeriodicTimer(TimeSpan.FromSeconds(10));
        var startTime = sut.GetUtcNow();
        var callbacks = new List<DateTimeOffset>();
        var callbacksTask = AsyncCallbacks(periodicTimer);

        sut.Advance(TimeSpan.FromSeconds(29));

        callbacks.Should().HaveCount(4);
        callbacks.Should().ContainInOrder(
            startTime + TimeSpan.FromSeconds(10),
            startTime + TimeSpan.FromSeconds(13),
            startTime + TimeSpan.FromSeconds(20),
            startTime + TimeSpan.FromSeconds(23));

        periodicTimer.Dispose();
        await callbacksTask;

        async Task AsyncCallbacks(PeriodicTimerWrapper periodicTimer)
        {
            while (await periodicTimer.WaitForNextTickAsync().ConfigureAwait(false))
            {
                callbacks.Add(sut.GetUtcNow());
                await sut.Delay(TimeSpan.FromSeconds(3));
                callbacks.Add(sut.GetUtcNow());
            }
        }
    }
#elif NET8_0_OR_GREATER
    [Fact]
    public async Task Callbacks_happens_in_schedule_order()
    {
        var sut = new ManualTimeProvider();
        var periodicTimer = sut.CreatePeriodicTimer(TimeSpan.FromSeconds(10));
        var startTime = sut.GetUtcNow();
        var callbacks = new List<DateTimeOffset>();
        var callbacksTask = AsyncCallbacks(periodicTimer);

        sut.Advance(TimeSpan.FromSeconds(29));

        callbacks.Should().HaveCount(4);
        callbacks.Should().ContainInOrder(
            startTime + TimeSpan.FromSeconds(10),
            startTime + TimeSpan.FromSeconds(13),
            startTime + TimeSpan.FromSeconds(20),
            startTime + TimeSpan.FromSeconds(23));

        periodicTimer.Dispose();
        await callbacksTask;

        async Task AsyncCallbacks(PeriodicTimer periodicTimer)
        {
            while (await periodicTimer.WaitForNextTickAsync().ConfigureAwait(false))
            {
                callbacks.Add(sut.GetUtcNow());
                await sut.Delay(TimeSpan.FromSeconds(3));
                callbacks.Add(sut.GetUtcNow());
            }
        }
    }
#endif

    [Fact]
    public void Timer_callback_GetUtcNow_AutoAdvance()
    {
        var oneSecond = TimeSpan.FromSeconds(1);
        var sut = new ManualTimeProvider() { AutoAdvanceBehavior = { UtcNowAdvanceAmount = oneSecond } };

        using var t1 = sut.CreateTimer(_ =>
        {
            sut.GetUtcNow();
        }, null, TimeSpan.Zero, oneSecond);

        sut.GetUtcNow().Should().Be(sut.Start + oneSecond);
    }

    [Fact]
    public void GetUtcNow_with_ClockAdvanceAmount_gt_zero()
    {
        var sut = new ManualTimeProvider() { AutoAdvanceBehavior = { UtcNowAdvanceAmount = 1.Seconds() } };

        var result = sut.GetUtcNow();

        result.Should().Be(sut.Start);
        sut.GetUtcNow().Should().Be(sut.Start + 1.Seconds());
    }

    [Fact]
    public void GetLocalNow_with_ClockAdvanceAmount_gt_zero()
    {
        var sut = new ManualTimeProvider() { AutoAdvanceBehavior = { UtcNowAdvanceAmount = 1.Seconds() } };

        var result = sut.GetLocalNow();

        result.Should().Be(sut.Start);
        sut.GetLocalNow().Should().Be(sut.Start + 1.Seconds());
    }

    [Fact]
    public void GetTimestamp_with_TimestampAdvanceAmount_gt_zero()
    {
        var sut = new ManualTimeProvider() { AutoAdvanceBehavior = { TimestampAdvanceAmount = 1.Seconds() } };

        var result = sut.GetTimestamp();

        result.Should().Be(sut.Start.Ticks);
        sut.GetTimestamp().Should().Be(result + 1.Seconds().Ticks);
    }

    [Fact]
    public void GetElapsedTime_with_TimestampAdvanceAmount_gt_zero()
    {
        var sut = new ManualTimeProvider() { AutoAdvanceBehavior = { TimestampAdvanceAmount = 1.Seconds() } };
        var start = sut.Start.Ticks;

        var result = sut.GetElapsedTime(start);

        result.Should().Be(0.Seconds());
        sut.GetElapsedTime(start).Should().Be(1.Seconds());
    }

    [Fact]
    public void Advance_zero()
    {
        var sut = new ManualTimeProvider();

        sut.Advance(TimeSpan.Zero);

        sut.GetUtcNow().Should().Be(sut.Start);
    }

    [Fact]
    public void Jump_zero()
    {
        var sut = new ManualTimeProvider();

        sut.Jump(TimeSpan.Zero);

        sut.GetUtcNow().Should().Be(sut.Start);
    }

    [Fact]
    public void Jump_throws_when_lt_zero()
    {
        var sut = new ManualTimeProvider();

        var throws = () => sut.Jump(TimeSpan.FromTicks(-1));

        throws.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Jump_throws_going_back_in_time()
    {
        var sut = new ManualTimeProvider();

        var throws = () => sut.Jump(sut.GetUtcNow() - TimeSpan.FromTicks(1));

        throws.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task Multi_threaded_SetUtcNow()
    {
        var callbackCount = 0;
        var sut = new ManualTimeProvider();
        using var timer = sut.CreateTimer(_ =>
        {
            Thread.Sleep(20);
            callbackCount++;
        }, null, 1.Seconds(), 1.Seconds());

        var tasks = Enumerable.Range(1, 100).Select(_ => Task.Run(() => sut.SetUtcNow(sut.Start + 1.Seconds())));

        await Task.WhenAll(tasks).ConfigureAwait(false);
        callbackCount.Should().Be(1);
    }

    [Fact]
    public async Task Multi_threaded_Jump()
    {
        var callbackCount = 0;
        var sut = new ManualTimeProvider();
        using var timer = sut.CreateTimer(_ =>
        {
            Thread.Sleep(20);
            callbackCount++;
        }, null, 1.Seconds(), 1.Seconds());

        var tasks = Enumerable.Range(1, 100).Select(_ => Task.Run(() => sut.Jump(sut.Start + 1.Seconds())));

        await Task.WhenAll(tasks).ConfigureAwait(false);
        callbackCount.Should().Be(1);
    }

    [Fact]
    public void ActiveTimers_with_no_timers()
    {
        var sut = new ManualTimeProvider();

        sut.ActiveTimers.Should().Be(0);
    }

    [Fact]
    public void ActiveTimers_with_active_timers()
    {
        var sut = new ManualTimeProvider();

        var timer = sut.CreateTimer(_ => { }, null, 1.Seconds(), Timeout.InfiniteTimeSpan);

        sut.ActiveTimers.Should().Be(1);
    }

    [Fact]
    public void ActiveTimers_with_inactive_timers()
    {
        var sut = new ManualTimeProvider();

        var timer = sut.CreateTimer(_ => { }, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

        sut.ActiveTimers.Should().Be(0);
    }

    [Fact]
    public void ActiveTimers_with_after_timer_state_change()
    {
        var sut = new ManualTimeProvider();

        var timer = sut.CreateTimer(_ => { }, null, 1.Seconds(), Timeout.InfiniteTimeSpan);
        sut.Advance(1.Seconds());

        sut.ActiveTimers.Should().Be(0);
    }

    [Fact]
    public void CreateManualTimer_with_custom_timer_type()
    {
        var sut = new CustomManualTimeProvider();

        using var timer = sut.CreateTimer(_ => { }, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

        timer.Should().BeOfType<CustomManualTimer>();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public void Active_timer_with_TimerAutoAdvanceTimes_gt_zero(int timerAutoTriggerCount)
    {
        var sut = new ManualTimeProvider { AutoAdvanceBehavior = { TimerAutoTriggerCount = timerAutoTriggerCount } };
        var callbackTimes = 0;

        using var timer = sut.CreateTimer(_ => callbackTimes++, null, 1.Seconds(), 1.Seconds());

        callbackTimes.Should().Be(timerAutoTriggerCount);
        sut.GetUtcNow().Should().Be(sut.Start + TimeSpan.FromSeconds(timerAutoTriggerCount));
        sut.ActiveTimers.Should().Be(1);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public void Inactive_timer_with_TimerAutoAdvanceTimes_gt_zero(int timerAutoTriggerCount)
    {
        var sut = new ManualTimeProvider { AutoAdvanceBehavior = { TimerAutoTriggerCount = timerAutoTriggerCount } };
        var callbackTimes = 0;

        using var timer = sut.CreateTimer(_ => callbackTimes++, null, Timeout.InfiniteTimeSpan, 1.Seconds());

        callbackTimes.Should().Be(0);
        sut.GetUtcNow().Should().Be(sut.Start);
        sut.ActiveTimers.Should().Be(0);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public void Starting_timer_with_TimerAutoAdvanceTimes_gt_zero(int timerAutoTriggerCount)
    {
        var sut = new ManualTimeProvider { AutoAdvanceBehavior = { TimerAutoTriggerCount = timerAutoTriggerCount } };
        var callbackTimes = 0;
        using var timer = sut.CreateTimer(_ => callbackTimes++, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

        timer.Change(1.Seconds(), 1.Seconds());

        callbackTimes.Should().Be(timerAutoTriggerCount);
        sut.GetUtcNow().Should().Be(sut.Start + TimeSpan.FromSeconds(timerAutoTriggerCount));
        sut.ActiveTimers.Should().Be(1);
    }

    [Fact]
    public void Multiple_one_of_timers_with_TimerAutoAdvanceTimes_gt_zero()
    {
        var sut = new ManualTimeProvider { AutoAdvanceBehavior = { TimerAutoTriggerCount = 1 } };
        var timer1CallbackTimes = 0;
        var timer2CallbackTimes = 0;

        using var timer1 = sut.CreateTimer(_ => timer1CallbackTimes++, null, 1.Seconds(), Timeout.InfiniteTimeSpan);
        using var timer2 = sut.CreateTimer(_ => timer2CallbackTimes++, null, 2.Seconds(), Timeout.InfiniteTimeSpan);

        sut.ActiveTimers.Should().Be(0);
        sut.GetUtcNow().Should().Be(sut.Start + 1.Seconds() + 2.Seconds());
        timer1CallbackTimes.Should().Be(1);
        timer2CallbackTimes.Should().Be(1);
    }

    [Theory]
    [InlineData(1, 3, 1, 3)]
    [InlineData(10, 30, 10, 10 + 20)]
    public void Multiple_periodic_timers_with_TimerAutoAdvanceTimes_gt_zero(int timerAutoTriggerCount, int timer1ExpectedCallbackCount, int timer2ExpectedCallbackCount, int expectedSecondsSpend)
    {
        var sut = new ManualTimeProvider { AutoAdvanceBehavior = { TimerAutoTriggerCount = timerAutoTriggerCount } };
        var timer1CallbackTimes = 0;
        var timer2CallbackTimes = 0;

        using var timer1 = sut.CreateTimer(_ => timer1CallbackTimes++, null, 1.Seconds(), 1.Seconds());
        using var timer2 = sut.CreateTimer(_ => timer2CallbackTimes++, null, 2.Seconds(), 2.Seconds());

        sut.ActiveTimers.Should().Be(2);
        sut.GetUtcNow().Should().Be(sut.Start + expectedSecondsSpend.Seconds());
        timer1CallbackTimes.Should().Be(timer1ExpectedCallbackCount);
        timer2CallbackTimes.Should().Be(timer2ExpectedCallbackCount);
    }

    [Fact]
    public void Setting_AutoAdvanceBehavior_to_null()
    {
        var sut = new ManualTimeProvider();

        sut.AutoAdvanceBehavior = null!;

        sut.AutoAdvanceBehavior.Should().BeEquivalentTo(new AutoAdvanceBehavior());
    }

    private sealed class CustomManualTimeProvider : ManualTimeProvider
    {
        protected internal override ManualTimer CreateManualTimer(TimerCallback callback, object? state, ManualTimeProvider timeProvider)
            => new CustomManualTimer(callback, state, timeProvider);
    }

    private sealed class CustomManualTimer : ManualTimer
    {
        internal CustomManualTimer(TimerCallback callback, object? state, ManualTimeProvider timeProvider)
            : base(callback, state, timeProvider)
        {
        }
    }
}