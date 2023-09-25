namespace TimeProviderExtensions;

public class AutoAdvanceBehaviorTests
{
    [Fact]
    public void ClockAdvanceAmount_throws_when_lt_zero()
    {
        var sut = new AutoAdvanceBehavior();

        var throws = () => sut.UtcNowAdvanceAmount = TimeSpan.FromTicks(-1);

        throws.Should().Throw<ArgumentOutOfRangeException>()
            .And.ParamName.Should().Be(nameof(AutoAdvanceBehavior.UtcNowAdvanceAmount));
    }

    [Fact]
    public void TimestampAdvanceAmount_throws_when_lt_zero()
    {
        var sut = new AutoAdvanceBehavior();

        var throws = () => sut.TimestampAdvanceAmount = TimeSpan.FromTicks(-1);

        throws.Should().Throw<ArgumentOutOfRangeException>()
            .And.ParamName.Should().Be(nameof(AutoAdvanceBehavior.TimestampAdvanceAmount));
    }
}
