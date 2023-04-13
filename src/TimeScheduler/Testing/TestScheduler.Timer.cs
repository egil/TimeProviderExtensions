using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TimeScheduler.Testing;

public partial class TestScheduler : ITimeScheduler, IDisposable
{
    /// <inheritdoc/>
    public ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
        => new TestTimer(callback, state, dueTime, period, this);

    private sealed class TestTimer : ITimer
    {
        private readonly object? state;
        private readonly TestScheduler owner;

        private CancellationTokenSource stopTimerCts = new CancellationTokenSource();
        private TimeSpan currentDueTime;
        private TimeSpan currentPeriod;
        private TimerCallback? callback;
        private bool isDisposed;
        private bool running;

        public TestTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period, TestScheduler owner)
        {
            ValidateTimeSpanRange(dueTime);
            ValidateTimeSpanRange(period);

            this.callback = callback;
            this.state = state;
            this.currentDueTime = dueTime;
            this.currentPeriod = period;
            this.owner = owner;

            if (currentDueTime != Timeout.InfiniteTimeSpan)
            {
                ScheduleCallback(dueTime);
            }
        }

        public bool Change(TimeSpan dueTime, TimeSpan period)
        {
            ValidateTimeSpanRange(dueTime);
            ValidateTimeSpanRange(period);

            if (running)
            {
                stopTimerCts.Cancel();
                if (!stopTimerCts.TryReset())
                {
                    stopTimerCts = new();
                }
            }

            currentDueTime = dueTime;
            currentPeriod = period;

            if (currentDueTime != Timeout.InfiniteTimeSpan)
            {
                ScheduleCallback(dueTime);
            }

            return true;
        }

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }
            isDisposed = true;
            callback = null;
            stopTimerCts.Dispose();
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }

        private void TimerElapsed()
        {
            running = false;
            callback?.Invoke(state);

            if (currentPeriod != Timeout.InfiniteTimeSpan)
            {
                ScheduleCallback(currentPeriod);
            }
        }

        private void ScheduleCallback(TimeSpan waitTime)
        {
            running = true;
            owner.RegisterFutureAction(
                owner.UtcNow + waitTime,
                TimerElapsed,
                () => { },
                stopTimerCts.Token);
        }

        private static void ValidateTimeSpanRange(TimeSpan time, [CallerArgumentExpression("time")] string? parameter = null)
        {
            long tm = (long)time.TotalMilliseconds;
            if (tm < -1)
            {
                throw new ArgumentOutOfRangeException(parameter, $"{parameter}.TotalMilliseconds must be greater than -1.");
            }

            if (tm > TestScheduler.MaxSupportedTimeout)
            {
                throw new ArgumentOutOfRangeException(parameter, $"{parameter}.TotalMilliseconds must be less than than {TestScheduler.MaxSupportedTimeout}.");
            }
        }
    }
}
