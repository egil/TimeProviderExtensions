using System.Runtime.CompilerServices;

namespace TimeScheduler.Testing;

public partial class TestScheduler : ITimeScheduler
{
    /// <inheritdoc/>
    public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
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
#if NET6_0_OR_GREATER
                if (!stopTimerCts.TryReset())
                {
                    stopTimerCts = new();
                }
#else
                stopTimerCts = new();
#endif
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

#if NET6_0_OR_GREATER
        public ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }
#endif

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
            if (isDisposed)
            {
                return;
            }

            running = true;
            owner.RegisterFutureAction(
                owner.GetUtcNow() + waitTime,
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
