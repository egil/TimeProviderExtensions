using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeScheduler;

public partial class DefaultScheduler : ITimeScheduler
{
    /// <inheritdoc/>
    public ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
        => new TimerWrapper(callback, state, dueTime, period);

    private sealed class TimerWrapper : ITimer
    {
        private readonly Timer timer;

        public TimerWrapper(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
        {
            timer = new Timer(callback, state, dueTime, period);
        }

        public bool Change(TimeSpan dueTime, TimeSpan period) => timer.Change(dueTime, period);

        public void Dispose() => timer.Dispose();

        public ValueTask DisposeAsync() => timer.DisposeAsync();
    }
}
