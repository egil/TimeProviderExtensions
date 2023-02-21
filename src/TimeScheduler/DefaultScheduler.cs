using System.Threading;

namespace TimeScheduler;

public partial class DefaultScheduler : ITimeScheduler
{
    /// <inheritdoc/>
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

    /// <inheritdoc/>
    public Task Delay(TimeSpan delay)
        => Task.Delay(delay);

    /// <inheritdoc/>
    public Task Delay(TimeSpan delay, CancellationToken cancellationToken)
        => Task.Delay(delay, cancellationToken);

    /// <inheritdoc/>
    public PeriodicTimer PeriodicTimer(TimeSpan period)
        => new PeriodicTimerWrapper(period);

    /// <inheritdoc/>
    public Task WaitAsync(Task task, TimeSpan timeout)
    {
        ArgumentNullException.ThrowIfNull(task);
        return task.WaitAsync(timeout);
    }

    /// <inheritdoc/>
    public Task WaitAsync(Task task, TimeSpan timeout, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(task);
        return task.WaitAsync(timeout, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<TResult> WaitAsync<TResult>(Task<TResult> task, TimeSpan timeout)
    {
        ArgumentNullException.ThrowIfNull(task);
        return task.WaitAsync(timeout);
    }

    /// <inheritdoc/>
    public Task<TResult> WaitAsync<TResult>(Task<TResult> task, TimeSpan timeout, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(task);
        return task.WaitAsync(timeout, cancellationToken);
    }
}
