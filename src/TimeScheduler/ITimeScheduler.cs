using System;

namespace TimeScheduler;

public interface ITimeScheduler
{
    /// <summary>
    /// Returns a DateTimeOffset representing the current date and time.
    /// The resolution of the returned value depends on the system timer.
    /// Use this property as a replacement for <see cref="System.DateTimeOffset.UtcNow"/>.
    /// </summary>
    DateTimeOffset UtcNow { get; }

    /// <summary>
    /// Creates a Task that will complete after a time delay.
    /// Use this method as a replacement for <see cref="System.Threading.Tasks.Task.Delay(TimeSpan)"/>.
    /// </summary>
    /// <param name="delay">The time span to wait before completing the returned Task</param>
    /// <returns>A Task that represents the time delay</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// The <paramref name="delay"/> is less than -1 or greater than the maximum allowed timer duration.
    /// </exception>
    /// <remarks>
    /// After the specified time delay, the Task is completed in RanToCompletion state.
    /// </remarks>
    Task Delay(TimeSpan delay);

    /// <summary>
    /// Creates a Task that will complete after a time delay.
    /// Use this method as a replacement for <see cref="System.Threading.Tasks.Task.Delay(TimeSpan, CancellationToken)"/>.
    /// </summary>
    /// <param name="delay">The time span to wait before completing the returned Task</param>
    /// <param name="cancellationToken">The cancellation token that will be checked prior to completing the returned Task</param>
    /// <returns>A Task that represents the time delay</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// The <paramref name="delay"/> is less than -1 or greater than the maximum allowed timer duration.
    /// </exception>
    /// <exception cref="System.ObjectDisposedException">
    /// The <see cref="CancellationTokenSource"/> associated
    /// with <paramref name="cancellationToken"/> has already been disposed.
    /// </exception>
    /// <remarks>
    /// If the cancellation token is signaled before the specified time delay, then the Task is completed in
    /// Canceled state.  Otherwise, the Task is completed in RanToCompletion state once the specified time
    /// delay has expired.
    /// </remarks>
    Task Delay(TimeSpan delay, CancellationToken cancellationToken);

    /// <summary>
    /// Factory method that creates a periodic timer that enables waiting asynchronously for timer ticks.
    /// Use this factory method as a replacement for instantiating a <see cref="System.Threading.PeriodicTimer"/>.
    /// </summary>
    /// <remarks>
    /// This timer is intended to be used only by a single consumer at a time: only one call to <see cref="PeriodicTimer.WaitForNextTickAsync" />
    /// may be in flight at any given moment. <see cref="PeriodicTimer.Dispose"/> may be used concurrently with an active <see cref="PeriodicTimer.WaitForNextTickAsync"/>
    /// to interrupt it and cause it to return false.
    /// </remarks>
    PeriodicTimer PeriodicTimer(TimeSpan period);

    /// <summary>
    /// Gets a <see cref="Task"/> that will complete when this <see cref="Task"/> completes or when the specified timeout expires.
    /// Use this method as a replacement for <see cref="System.Threading.Tasks.Task.WaitAsync(TimeSpan)"/>.
    /// </summary>
    /// <param name="task">The task to wait for.</param>
    /// <param name="timeout">
    /// The timeout after which the <see cref="Task"/> should be faulted with a <see cref="TimeoutException"/> if it hasn't otherwise completed.
    /// </param>
    /// <returns>The <see cref="Task"/> representing the asynchronous wait. It may or may not be the same instance as the current instance.</returns>
    Task WaitAsync(Task task, TimeSpan timeout);

    /// <summary>
    /// Gets a <see cref="Task"/> that will complete when this <see cref="Task"/> completes, when the specified timeout expires, or when the specified <see cref="CancellationToken"/> has cancellation requested.
    /// Use this method as a replacement for <see cref="System.Threading.Tasks.Task.WaitAsync(TimeSpan, CancellationToken)"/>.
    /// </summary>
    /// <param name="task">The task to wait for.</param>
    /// <param name="timeout">The timeout after which the <see cref="Task"/> should be faulted with a <see cref="TimeoutException"/> if it hasn't otherwise completed.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for a cancellation request.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous wait.  It may or may not be the same instance as the current instance.</returns>
    Task WaitAsync(Task task, TimeSpan timeout, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a <see cref="Task{TResult}"/> that will complete when this <see cref="Task{TResult}"/> completes or when the specified timeout expires.
    /// Use this method as a replacement for <see cref="System.Threading.Tasks.Task{TResult}.WaitAsync(TimeSpan)"/>.
    /// </summary>
    /// <param name="task">The task to wait for.</param>
    /// <param name="timeout">
    /// The timeout after which the <see cref="Task"/> should be faulted with a <see cref="TimeoutException"/> if it hasn't otherwise completed.
    /// </param>
    /// <returns>The <see cref="Task{TResult}"/> representing the asynchronous wait. It may or may not be the same instance as the current instance.</returns>
    Task<TResult> WaitAsync<TResult>(Task<TResult> task, TimeSpan timeout);

    /// <summary>
    /// Gets a <see cref="Task{TResult}"/> that will complete when this <see cref="Task{TResult}"/> completes, when the specified timeout expires, or when the specified <see cref="CancellationToken"/> has cancellation requested.
    /// Use this method as a replacement for <see cref="System.Threading.Tasks.Task.WaitAsync(TimeSpan, CancellationToken)"/>.
    /// </summary>
    /// <param name="task">The task to wait for.</param>
    /// <param name="timeout">The timeout after which the <see cref="Task"/> should be faulted with a <see cref="TimeoutException"/> if it hasn't otherwise completed.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for a cancellation request.</param>
    /// <returns>The <see cref="Task{TResult}"/> representing the asynchronous wait. It may or may not be the same instance as the current instance.</returns>
    Task<TResult> WaitAsync<TResult>(Task<TResult> task, TimeSpan timeout, CancellationToken cancellationToken);
}
