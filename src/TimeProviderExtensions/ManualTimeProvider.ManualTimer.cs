using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TimeProviderExtensions;

/// <summary>
/// Represents a synthetic time provider that can be used to enable deterministic behavior in tests.
/// </summary>
/// <remarks>
/// Learn more at <see href="https://github.com/egil/TimeProviderExtensions"/>.
/// </remarks>
public partial class ManualTimeProvider : TimeProvider
{
    [DebuggerDisplay("ManualTimer: {scheduledCallback.ToString(),nq}, due time = {dueTime}, period = {period}.")]
    private sealed class ManualTimer : ITimer
    {
        private ManualTimerScheduler? scheduledCallback;
        private TimeSpan dueTime;
        private TimeSpan period;

        public ManualTimer(TimerCallback callback, object? state, ManualTimeProvider timeProvider)
        {
            scheduledCallback = new ManualTimerScheduler(timeProvider, callback, state);
        }

        /// <inheritdoc/>
        public bool Change(TimeSpan dueTime, TimeSpan period)
        {
            ValidateTimeSpanRange(dueTime);
            ValidateTimeSpanRange(period);

            if (scheduledCallback is null)
            {
                return false;
            }

            this.dueTime = dueTime;
            this.period = period;

            scheduledCallback.Change(dueTime, period);

            return true;
        }

        // In case the timer is not disposed explicitly by the user.
        ~ManualTimer() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public ValueTask DisposeAsync()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            return ValueTask.CompletedTask;
        }

        private void Dispose(bool _)
        {
            if (scheduledCallback is null)
            {
                return;
            }

            scheduledCallback.Cancel();
            scheduledCallback = null;
        }

        private static void ValidateTimeSpanRange(TimeSpan time, [CallerArgumentExpression("time")] string? parameter = null)
        {
            long tm = (long)time.TotalMilliseconds;
            if (tm < -1)
            {
                throw new ArgumentOutOfRangeException(parameter, $"{parameter}.TotalMilliseconds must be greater than -1.");
            }

            if (tm > MaxSupportedTimeout)
            {
                throw new ArgumentOutOfRangeException(parameter, $"{parameter}.TotalMilliseconds must be less than than {MaxSupportedTimeout}.");
            }
        }
    }
}
