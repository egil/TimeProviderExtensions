namespace TimeProviderExtensions;

public static class ManualTimeProviderExtensions
{
    /// <summary>
    /// Advance the date and time represented by <see cref="GetUtcNow()"/>
    /// by one millisecond, and triggers any scheduled items that are waiting for time to be forwarded.
    /// </summary>
    public static void Advance(this ManualTimeProvider timeProvider)
        => timeProvider.Advance(TimeSpan.FromMilliseconds(1));
}
