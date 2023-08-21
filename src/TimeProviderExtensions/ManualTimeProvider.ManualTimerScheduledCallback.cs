namespace TimeProviderExtensions;

/// <summary>
/// Represents a synthetic time provider that can be used to enable deterministic behavior in tests.
/// </summary>
/// <remarks>
/// Learn more at <see href="https://github.com/egil/TimeProviderExtensions"/>.
/// </remarks>
public partial class ManualTimeProvider : TimeProvider
{
    // The reason this class exists and it is separate from ManualTimer is that
    // the GC should be collect the ManualTimer in case users forget to dispose it.
    // If all the references captured by this type was part of the ManualTimer
    // type, the finalizer would not be invoked on ManualTimer if a callback was scheduled.
    private sealed class ManualTimerScheduler : IComparable<ManualTimerScheduler>
    {
        private readonly TimerCallback callback;
        private readonly object? state;
        private readonly ManualTimeProvider timeProvider;
        private TimeSpan period;
        private bool running;

        public DateTimeOffset CallbackTime { get; set; }

        public ManualTimerScheduler(ManualTimeProvider timeProvider, TimerCallback callback, object? state)
        {
            this.timeProvider = timeProvider;
            this.callback = callback;
            this.state = state;
        }

        public int CompareTo(ManualTimerScheduler? other)
            => other is not null
            ? Comparer<DateTimeOffset>.Default.Compare(CallbackTime, other.CallbackTime)
            : -1;

        internal void Cancel()
        {
            if (running)
            {
                timeProvider.RemoveCallback(this);
            }
        }

        internal void Change(TimeSpan dueTime, TimeSpan period)
        {
            Cancel();

            this.period = period;

            if (dueTime != Timeout.InfiniteTimeSpan)
            {
                ScheduleCallback(dueTime);
            }
        }

        internal void TimerElapsed()
        {
            running = false;

            callback.Invoke(state);

            if (period != Timeout.InfiniteTimeSpan && period != TimeSpan.Zero)
            {
                ScheduleCallback(period);
            }
        }

        private void ScheduleCallback(TimeSpan waitTime)
        {
            running = true;

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
}
