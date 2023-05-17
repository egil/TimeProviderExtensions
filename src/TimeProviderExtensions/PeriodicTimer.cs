#if NET6_0_OR_GREATER
namespace TimeProviderExtensions;

/// <summary>Provides a periodic timer that enables waiting asynchronously for timer ticks.</summary>
/// <remarks>
/// This timer is intended to be used only by a single consumer at a time: only one call to <see cref="WaitForNextTickAsync" />
/// may be in flight at any given moment.  <see cref="Dispose()"/> may be used concurrently with an active <see cref="WaitForNextTickAsync"/>
/// to interrupt it and cause it to return false.
/// </remarks>
public abstract class PeriodicTimer : IDisposable
{
    /// <summary>Wait for the next tick of the timer, or for the timer to be stopped.</summary>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to use to cancel the asynchronous wait. If cancellation is requested, it affects only the single wait operation;
    /// the underlying timer continues firing.
    /// </param>
    /// <returns>A task that will be completed due to the timer firing, <see cref="Dispose()"/> being called to stop the timer, or cancellation being requested.</returns>
    /// <remarks>
    /// The <see cref="PeriodicTimer"/> behaves like an auto-reset event, in that multiple ticks are coalesced into a single tick if they occur between
    /// calls to <see cref="WaitForNextTickAsync"/>.  Similarly, a call to <see cref="Dispose()"/> will void any tick not yet consumed. <see cref="WaitForNextTickAsync"/>
    /// may only be used by one consumer at a time, and may be used concurrently with a single call to <see cref="Dispose()"/>.
    /// </remarks>
    public abstract ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default);

    /// <summary>Stops the timer and releases associated managed resources.</summary>
    /// <remarks>
    /// <see cref="Dispose()"/> will cause an active wait with <see cref="WaitForNextTickAsync"/> to complete with a value of false.
    /// All subsequent <see cref="WaitForNextTickAsync"/> invocations will produce a value of false.
    /// </remarks>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Stops the timer and releases associated managed resources.
    /// </summary>
    /// <remarks>
    /// This method follows the dispose pattern.
    /// See <see href="https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose"/>
    /// to learn more.
    /// </remarks>
    protected virtual void Dispose(bool disposing)
    {
    }
}
#endif