namespace System.Testing;

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
}
