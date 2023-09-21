using System.Globalization;

namespace TimeProviderExtensions;

/// <summary>
/// The type that takes care of scheduling <see cref="ManualTimer"/> callbacks.
/// </summary>
/// <remarks>
/// The reason this class exists and it is separate from <see cref="ManualTimer"/> is that
/// the GC should be collect the <see cref="ManualTimer"/> in case users forget to dispose it.
/// If all the references captured by this type was part of the <see cref="ManualTimer"/>
/// type, the finalizer would not be invoked on <see cref="ManualTimer"/> if a callback was scheduled.
/// </remarks>
internal sealed class ManualTimerScheduler
{
    private readonly TimerCallback callback;
    private readonly object? state;
    private readonly ManualTimeProvider timeProvider;

    internal int CallbackInvokeCount { get; private set; }

    internal DateTimeOffset? CallbackTime { get; set; }

    internal TimeSpan Period { get; private set; }

    internal ManualTimerScheduler(ManualTimeProvider timeProvider, TimerCallback callback, object? state)
    {
        this.timeProvider = timeProvider;
        this.callback = callback;
        this.state = state;
    }

    internal void Cancel()
    {
        if (CallbackTime.HasValue)
        {
            CallbackTime = null;
            timeProvider.RemoveCallback(this);
        }
    }

    internal void Change(TimeSpan dueTime, TimeSpan period)
    {
        Cancel();

        Period = period;

        if (dueTime != Timeout.InfiniteTimeSpan)
        {
            ScheduleCallback(dueTime);
        }
    }

    internal void TimerElapsed()
    {
        CallbackTime = null;
        callback.Invoke(state);
        CallbackInvokeCount++;

        if (Period != Timeout.InfiniteTimeSpan && Period != TimeSpan.Zero)
        {
            ScheduleCallback(Period);
        }
    }

    private void ScheduleCallback(TimeSpan waitTime)
    {
        if (waitTime == TimeSpan.Zero)
        {
            TimerElapsed();
        }
        else
        {
            timeProvider.ScheduleCallback(this, waitTime);
        }
    }
}