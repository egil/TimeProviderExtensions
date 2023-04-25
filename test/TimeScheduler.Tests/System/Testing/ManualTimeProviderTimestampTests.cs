namespace System.Testing;

public class ManualTimeProviderTimestampTests
{
    [Fact]
    public void TimestampFrequency_ten_mill()
    {
        var sut = new ManualTimeProvider();

        sut.TimestampFrequency.Should().Be(10_000_000);
    }

    [Fact]
    public void GetTimestamp_increments_by_ticks()
    {
        var sut = new ManualTimeProvider();
        var timestamp = sut.GetTimestamp();

        sut.ForwardTime(TimeSpan.FromTicks(1));

        sut.GetTimestamp().Should().Be(timestamp + 1);
    }
}
