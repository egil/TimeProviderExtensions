using System;
using Xunit;
namespace System.Testing;

public class TestTimeProviderTimestampTests
{
    [Fact]
    public void TimestampFrequency_ten_mill()
    {
        using var sut = new TestTimeProvider();

        sut.TimestampFrequency.Should().Be(10_000_000);
    }

    [Fact]
    public void GetTimestamp_increments_by_ticks()
    {
        using var sut = new TestTimeProvider();
        var timestamp = sut.GetTimestamp();

        sut.ForwardTime(TimeSpan.FromTicks(1));

        sut.GetTimestamp().Should().Be(timestamp + 1);
    }
}
