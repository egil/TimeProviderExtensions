// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
//
// Copied from https://github.com/dotnet/runtime/blob/2d9cb2d33f6f89f0a1a01782f2e474051bb2894a/src/libraries/Microsoft.Bcl.TimeProvider/src/System/Threading/Tasks/TimeProviderTaskExtensions.cs
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace System.Threading.Tasks;

/// <summary>
/// Provide extensions methods for <see cref="Task"/> operations with <see cref="TimeProvider"/>.
/// </summary>
public static class TimeProviderTaskExtensions
{
    private sealed class DelayState : TaskCompletionSource<bool>
    {
        // The original code passed TaskCreationOptions.RunContinuationsAsynchronously to the base constructor,
        // that has been removed for testing purposes since running code asynchronously makes testing more difficult.
        // When continuations are invoked synchronously, the test code can be written in a more linear fashion,
        // without having to add an `Task.Delay(1)` after a call to `ForwardTime` to ensure that the continuation.
        public DelayState() : base() { }
        public ITimer Timer { get; set; }
        public CancellationTokenRegistration Registration { get; set; }
    }

#if NET6_0_OR_GREATER
    private sealed class WaitAsyncState : TaskCompletionSource<bool>
    {
        // The original code passed TaskCreationOptions.RunContinuationsAsynchronously to the base constructor,
        // that has been removed for testing purposes since running code asynchronously makes testing more difficult.
        // When continuations are invoked synchronously, the test code can be written in a more linear fashion,
        // without having to add an `Task.Delay(1)` after a call to `ForwardTime` to ensure that the continuation.
        public WaitAsyncState() : base() { }
        public readonly CancellationTokenSource ContinuationCancellation = new CancellationTokenSource();
        public CancellationTokenRegistration Registration;
        public ITimer? Timer;
    }
#endif

    /// <summary>Creates a task that completes after a specified time interval.</summary>
    /// <param name="timeProvider">The <see cref="TimeProvider"/> with which to interpret <paramref name="delay"/>.</param>
    /// <param name="delay">The <see cref="TimeSpan"/> to wait before completing the returned task, or <see cref="Timeout.InfiniteTimeSpan"/> to wait indefinitely.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the time delay.</returns>
    /// <exception cref="System.ArgumentNullException">The <paramref name="timeProvider"/> argument is null.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="delay"/> represents a negative time interval other than <see cref="Timeout.InfiniteTimeSpan"/>.</exception>
    public static Task Delay(this TimeProvider timeProvider, TimeSpan delay, CancellationToken cancellationToken = default)
    {
        if (timeProvider == TimeProvider.System)
        {
            return Task.Delay(delay, cancellationToken);
        }

        if (timeProvider is null)
        {
            throw new ArgumentNullException(nameof(timeProvider));
        }

        if (delay != Timeout.InfiniteTimeSpan && delay < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(delay));
        }

        if (delay == TimeSpan.Zero)
        {
            return Task.CompletedTask;
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(cancellationToken);
        }

        DelayState state = new();

        // To prevent a race condition where the timer may fire before being assigned to s.Timer,
        // we initialize it with an InfiniteTimeSpan and then set it to the state variable, followed by calling Time.Change.
        state.Timer = timeProvider.CreateTimer(delayState =>
        {
            DelayState s = (DelayState)delayState;
            s.TrySetResult(true);
            s.Registration.Dispose();
            s.Timer.Dispose();
        }, state, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

        state.Timer.Change(delay, Timeout.InfiniteTimeSpan);

        state.Registration = cancellationToken.Register(delayState =>
        {
            DelayState s = (DelayState)delayState;
            s.TrySetCanceled(cancellationToken);
            s.Timer.Dispose();
            s.Registration.Dispose();
        }, state);

        // To prevent a race condition where the timer fires after we have attached the cancellation callback
        // but before the registration is stored in state.Registration, we perform a subsequent check to ensure
        // that the registration is not left dangling.
        if (state.Task.IsCompleted)
        {
#pragma warning disable CA1849 // Call async methods when in an async method
            state.Registration.Dispose();
#pragma warning restore CA1849 // Call async methods when in an async method
        }

        return state.Task;
    }

#if NET6_0_OR_GREATER

    /// <summary>
    /// Gets a <see cref="Task"/> that will complete when this <see cref="Task"/> completes, when the specified timeout expires, or when the specified <see cref="CancellationToken"/> has cancellation requested.
    /// </summary>
    /// <param name="task">The task for which to wait on until completion.</param>
    /// <param name="timeout">The timeout after which the <see cref="Task"/> should be faulted with a <see cref="TimeoutException"/> if it hasn't otherwise completed.</param>
    /// <param name="timeProvider">The <see cref="TimeProvider"/> with which to interpret <paramref name="timeout"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for a cancellation request.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous wait.  It may or may not be the same instance as the current instance.</returns>
    /// <exception cref="System.ArgumentNullException">The <paramref name="task"/> argument is null.</exception>
    /// <exception cref="System.ArgumentNullException">The <paramref name="timeProvider"/> argument is null.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="timeout"/> represents a negative time interval other than <see cref="Timeout.InfiniteTimeSpan"/>.</exception>
    public static Task WaitAsync(this Task task, TimeSpan timeout, TimeProvider timeProvider, CancellationToken cancellationToken = default)
    {
        if (task is null)
        {
            throw new ArgumentNullException(nameof(task));
        }

        if (timeout != Timeout.InfiniteTimeSpan && timeout < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout));
        }

        if (timeProvider is null)
        {
            throw new ArgumentNullException(nameof(timeProvider));
        }

        if (task.IsCompleted)
        {
            return task;
        }

        if (timeout == Timeout.InfiniteTimeSpan && !cancellationToken.CanBeCanceled)
        {
            return task;
        }

        if (timeout == TimeSpan.Zero)
        {
            return Task.FromException(new TimeoutException());
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(cancellationToken);
        }

        var state = new WaitAsyncState();

        // To prevent a race condition where the timer may fire before being assigned to s.Timer,
        // we initialize it with an InfiniteTimeSpan and then set it to the state variable, followed by calling Time.Change.
        state.Timer = timeProvider.CreateTimer(static s =>
        {
            var state = (WaitAsyncState)s!;

            state.TrySetException(new TimeoutException());

            state.Registration.Dispose();
            state.Timer!.Dispose();
            state.ContinuationCancellation.Cancel();
        }, state, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        state.Timer.Change(timeout, Timeout.InfiniteTimeSpan);

        _ = task.ContinueWith(static (t, s) =>
        {
            var state = (WaitAsyncState)s!;

            if (t.IsFaulted) state.TrySetException(t.Exception.InnerExceptions);
            else if (t.IsCanceled) state.TrySetCanceled();
            else state.TrySetResult(true);

            state.Registration.Dispose();
            state.Timer?.Dispose();
        }, state, state.ContinuationCancellation.Token, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

        state.Registration = cancellationToken.Register(static s =>
        {
            var state = (WaitAsyncState)s!;

            state.TrySetCanceled();

            state.Timer?.Dispose();
            state.ContinuationCancellation.Cancel();
        }, state);

        // To prevent a race condition where the timer fires after we have attached the cancellation callback
        // but before the registration is stored in state.Registration, we perform a subsequent check to ensure
        // that the registration is not left dangling.
        if (state.Task.IsCompleted)
        {
#pragma warning disable CA1849 // Call async methods when in an async method
            state.Registration.Dispose();
#pragma warning restore CA1849 // Call async methods when in an async method
        }

        return state.Task;
    }

    /// <summary>
    /// Gets a <see cref="Task"/> that will complete when this <see cref="Task"/> completes, when the specified timeout expires, or when the specified <see cref="CancellationToken"/> has cancellation requested.
    /// </summary>
    /// <param name="task">The task for which to wait on until completion.</param>
    /// <param name="timeout">The timeout after which the <see cref="Task"/> should be faulted with a <see cref="TimeoutException"/> if it hasn't otherwise completed.</param>
    /// <param name="timeProvider">The <see cref="TimeProvider"/> with which to interpret <paramref name="timeout"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for a cancellation request.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous wait.  It may or may not be the same instance as the current instance.</returns>
    /// <exception cref="System.ArgumentNullException">The <paramref name="task"/> argument is null.</exception>
    /// <exception cref="System.ArgumentNullException">The <paramref name="timeProvider"/> argument is null.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="timeout"/> represents a negative time interval other than <see cref="Timeout.InfiniteTimeSpan"/>.</exception>
    public static async Task<TResult> WaitAsync<TResult>(this Task<TResult> task, TimeSpan timeout, TimeProvider timeProvider, CancellationToken cancellationToken = default)
    {
        await ((Task)task).WaitAsync(timeout, timeProvider, cancellationToken).ConfigureAwait(false);
#pragma warning disable CA1849 // Call async methods when in an async method
        return task.Result;
#pragma warning restore CA1849 // Call async methods when in an async method
    }

#endif
}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.