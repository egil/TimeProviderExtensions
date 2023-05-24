#if TargetMicrosoftTestTimeProvider && !RELEASE
using SutTimeProvider = Microsoft.Extensions.Time.Testing.FakeTimeProvider;
#else
using SutTimeProvider = TimeProviderExtensions.ManualTimeProvider;
#endif

namespace TimeProviderExtensions;

public class ManualTimeProviderTimestampTests
{
    [Fact]
    public void TimestampFrequency_ten_mill()
    {
        var sut = new SutTimeProvider();

        sut.TimestampFrequency.Should().Be(10_000_000);
    }

    [Fact]
    public void GetTimestamp_increments_by_ticks()
    {
        var sut = new SutTimeProvider();
        var timestamp = sut.GetTimestamp();

        sut.Advance(TimeSpan.FromTicks(1));

        sut.GetTimestamp().Should().Be(timestamp + 1);
    }
}
