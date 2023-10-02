#if NET6_0_OR_GREATER && !NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
using TimeProviderExtensions;

namespace System.Threading;

/// <summary>
/// Provides a lightweight wrapper around a <see cref="PeriodicTimer"/> to enable controlling the timer via a <see cref="TimeProvider"/>.
/// A periodic timer enables waiting asynchronously for timer ticks.
/// </summary>
/// <remarks>
/// <para>
/// This timer is intended to be used only by a single consumer at a time: only one call to <see cref="WaitForNextTickAsync" />
/// may be in flight at any given moment.  <see cref="Dispose()"/> may be used concurrently with an active <see cref="WaitForNextTickAsync"/>
/// to interrupt it and cause it to return false.
/// </para>
/// </remarks>
[ExcludeFromCodeCoverage]
public abstract class PeriodicTimerWrapper : IDisposable
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

    /// <summary>Ensures that resources are freed and other cleanup operations are performed when the garbage collector reclaims the <see cref="PeriodicTimer" /> object.</summary>
    ~PeriodicTimerWrapper() => Dispose(false);

    /// <summary>Dispose of the wrapped <see cref="PeriodicTimer"/>.</summary>
    protected abstract void Dispose(bool disposing);

    internal sealed class PeriodicTimerPortWrapper : PeriodicTimerWrapper
    {
        private readonly PeriodicTimerPort periodicTimer;

        public PeriodicTimerPortWrapper(PeriodicTimerPort periodicTimer)
        {
            this.periodicTimer = periodicTimer;
        }

        protected override void Dispose(bool disposing)
            => periodicTimer.Dispose();

        public override ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default)
            => periodicTimer.WaitForNextTickAsync(cancellationToken);
    }

    internal sealed class PeriodicTimerOrgWrapper : PeriodicTimerWrapper
    {
        private readonly PeriodicTimer periodicTimer;

        public PeriodicTimerOrgWrapper(PeriodicTimer periodicTimer)
        {
            this.periodicTimer = periodicTimer;
        }

        protected override void Dispose(bool disposing)
            => periodicTimer.Dispose();

        public override ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default)
            => periodicTimer.WaitForNextTickAsync(cancellationToken);
    }
}
#endif