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
        var timeProvider = new ManualTimeProvider() { AutoAdvanceAmount = oneSecond };

        var t1 = timeProvider.CreateTimer(_ =>
        {
            timeProvider.GetUtcNow();
        }, null, TimeSpan.Zero, oneSecond);

        timeProvider.GetUtcNow().Should().Be(timeProvider.Start + oneSecond);
    }
}