#if TargetMicrosoftTestTimeProvider
using SutTimeProvider = Microsoft.Extensions.Time.Testing.FakeTimeProvider;
#else
using SutTimeProvider = TimeProviderExtensions.ManualTimeProvider;
#endif

namespace TimeProviderExtensions;

public class ManualTimeProviderTests
{
    [Fact]
    public void Advance_updates_UtcNow()
    {
        var startTime = DateTimeOffset.UtcNow;
        var sut = new SutTimeProvider(startTime);

        sut.Advance(TimeSpan.FromTicks(1));

        sut.GetUtcNow().Should().Be(startTime + TimeSpan.FromTicks(1));
    }

    [Fact]
    public void SetUtcNow_updates_UtcNow()
    {
        var startTime = DateTimeOffset.UtcNow;
        var sut = new SutTimeProvider(startTime);

        sut.SetUtcNow(startTime + TimeSpan.FromTicks(1));

        sut.GetUtcNow().Should().Be(startTime + TimeSpan.FromTicks(1));
    }
#if NET8_0_OR_GREATER
    [Fact]
    public async Task Delay_callbacks_runs_synchronously()
    {
        // arrange
        var sut = new SutTimeProvider();
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

    [Fact]
    public async Task WaitAsync_callbacks_runs_synchronously()
    {
        // arrange
        var sut = new SutTimeProvider();
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
                await Task.Delay(TimeSpan.FromDays(1))
                    .WaitAsync(TimeSpan.FromSeconds(10), timeProvider);
            }
            catch (TimeoutException)
            {
                callback();
            }
        }
    }
#endif
}
