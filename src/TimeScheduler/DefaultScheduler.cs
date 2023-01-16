namespace TimeScheduler;

public partial class DefaultScheduler : ITimeScheduler
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

    public Task Delay(TimeSpan delay)
        => Task.Delay(delay);

    public Task Delay(TimeSpan delay, CancellationToken cancellationToken)
        => Task.Delay(delay, cancellationToken);

    public PeriodicTimer PeriodicTimer(TimeSpan period)
        => new PeriodicTimerWrapper(period);
}
