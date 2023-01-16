namespace TimeScheduler.Testing;

public partial class TestScheduler : ITimeScheduler, IDisposable
{
    private sealed class TestPeriodicTimer : PeriodicTimer
    {
        private readonly TimeSpan period;
        private readonly TestScheduler owner;
        private bool stopped;
        private TaskCompletionSource<bool>? completionSource;
        private DateTimeOffset nextSignal;

        public TestPeriodicTimer(TimeSpan period, TestScheduler owner)
        {
            this.period = period;
            this.owner = owner;
            SetNextSignalTime();

        }

        public override ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default)
        {
            if (completionSource is not null)
            {
                throw new InvalidOperationException("WaitForNextTickAsync should only be used by one consumer at a time. Failing to do so is an error.");
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return ValueTask.FromCanceled<bool>(cancellationToken);
            }

            if (!stopped && owner.UtcNow >= nextSignal)
            {
                SetNextSignalTime();
                return new ValueTask<bool>(!stopped);
            }

            completionSource = new TaskCompletionSource<bool>();

            owner.RegisterDelayedTask(
                nextSignal,
                () => Signal(),
                () => completionSource?.TrySetCanceled(),
                cancellationToken);

            return new ValueTask<bool>(completionSource.Task);
        }

        private void Signal()
        {
            if (completionSource is not null)
            {
                completionSource.TrySetResult(!stopped);
                completionSource = null;
            }

            SetNextSignalTime();
        }

        private void SetNextSignalTime()
        {
            nextSignal = owner.UtcNow + period;
        }

        protected override void Dispose(bool disposing)
        {
            stopped = true;
            Signal();
            base.Dispose(disposing);
        }
    }
}
