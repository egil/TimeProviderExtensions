using System.Runtime.CompilerServices;

namespace TimeScheduler;

/// <summary>
/// Represents a default implementation of a <see cref="ITimeScheduler"/>,
/// that simply wraps built-in types, static methods, and static properties
/// in .NET framework.
/// </summary>
/// <remarks>
/// Learn more at <see href="https://github.com/egil/TimeScheduler"/>.
/// </remarks>
public partial class DefaultScheduler : TimeProvider, ITimeScheduler
{
    /// <summary>
    /// Gets a singleton instance of the <see cref="DefaultScheduler"/>.
    /// </summary>
    public static DefaultScheduler Instance { get; } = new DefaultScheduler();

    /// <inheritdoc/>
    [Obsolete("Use GetUtcNow() instead. This will allow you to upgrade seamlessly to TimeProvider that will be part of .NET 8.")]
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override DateTimeOffset GetUtcNow() => base.GetUtcNow();

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CancelAfter(CancellationTokenSource cancellationTokenSource, TimeSpan delay)
    {
        if (cancellationTokenSource is null)
        {
            throw new ArgumentNullException(nameof(cancellationTokenSource));
        }

        cancellationTokenSource.CancelAfter(delay);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task Delay(TimeSpan delay)
        => Task.Delay(delay);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task Delay(TimeSpan delay, CancellationToken cancellationToken)
        => Task.Delay(delay, cancellationToken);

#if NET6_0_OR_GREATER
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PeriodicTimer PeriodicTimer(TimeSpan period)
        => new PeriodicTimerWrapper(period);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task WaitAsync(Task task, TimeSpan timeout)
    {
        if (task is null)
        {
            throw new ArgumentNullException(nameof(task));
        }

        return task.WaitAsync(timeout);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task WaitAsync(Task task, TimeSpan timeout, CancellationToken cancellationToken)
    {
        if (task is null)
        {
            throw new ArgumentNullException(nameof(task));
        }

        return task.WaitAsync(timeout, cancellationToken);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<TResult> WaitAsync<TResult>(Task<TResult> task, TimeSpan timeout)
    {
        if (task is null)
        {
            throw new ArgumentNullException(nameof(task));
        }

        return task.WaitAsync(timeout);
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<TResult> WaitAsync<TResult>(Task<TResult> task, TimeSpan timeout, CancellationToken cancellationToken)
    {
        if (task is null)
        {
            throw new ArgumentNullException(nameof(task));
        }

        return task.WaitAsync(timeout, cancellationToken);
    }
#endif
}
