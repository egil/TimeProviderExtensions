#if NET6_0_OR_GREATER
namespace TimeScheduler;

internal sealed class PeriodicTimerWrapper : PeriodicTimer
{
    private readonly System.Threading.PeriodicTimer timer;

    public PeriodicTimerWrapper(TimeSpan period)
    {
        timer = new System.Threading.PeriodicTimer(period);
    }

    public override ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default)
        => timer.WaitForNextTickAsync(cancellationToken);

    protected override void Dispose(bool disposing)
    {
        timer.Dispose();
        base.Dispose(disposing);
    }
}
#endif