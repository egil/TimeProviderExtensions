namespace TimeProviderExtensions;

public class ManualTimeProviderTests
{
    [Fact]
    public void ForwardTime_updates_UtcNow()
    {
        var startTime = DateTimeOffset.UtcNow;
        var sut = new ManualTimeProvider(startTime);

        sut.ForwardTime(TimeSpan.FromTicks(1));

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
    public void Delay_callbacks_runs_synchronously()
    {
        // arrange
        var sut = new ManualTimeProvider();
        var callbackCount = 0;
        _ = Continuation(sut, () => callbackCount++);

        // act
        sut.ForwardTime(TimeSpan.FromSeconds(10));

        // assert
        callbackCount.Should().Be(1);

        static async Task Continuation(TimeProvider timeProvider, Action callback)
        {
            await timeProvider.Delay(TimeSpan.FromSeconds(10));
            callback();
        }
    }

    [Fact]
    public void WaitAsync_callbacks_runs_synchronously()
    {
        // arrange
        var sut = new ManualTimeProvider();
        var callbackCount = 0;
        _ = Continuation(sut, () => callbackCount++);

        // act
        sut.ForwardTime(TimeSpan.FromSeconds(10));

        // assert
        callbackCount.Should().Be(1);

        static async Task Continuation(TimeProvider timeProvider, Action callback)
        {
            try
            {
                await Task.Delay(TimeSpan.FromDays(1)).WaitAsync(TimeSpan.FromSeconds(10), timeProvider);
            }
            catch (TimeoutException)
            {
                callback();
            }
        }
    }
}
