using System;

namespace Scheduler;

public interface ITimeScheduler
{
    DateTimeOffset UtcNow { get; }

    Task Delay(TimeSpan delay);

    Task Delay(TimeSpan delay, CancellationToken cancellationToken);

    PeriodicTimer PeriodicTimer(TimeSpan period);
}
