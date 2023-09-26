[![GitHub tag (latest SemVer pre-release)](https://img.shields.io/github/v/tag/egil/TimeProviderExtensions?include_prereleases&logo=github&style=flat-square)](https://github.com/egil/TimeProviderExtensions/releases)
[![Nuget](https://img.shields.io/nuget/dt/TimeProviderExtensions?logo=nuget&style=flat-square)](https://www.nuget.org/packages/TimeProviderExtensions/)
[![Issues Open](https://img.shields.io/github/issues/egil/TimeProviderExtensions.svg?style=flat-square&logo=github)](https://github.com/egil/TimeProviderExtensions/issues)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2Fegil%2FTimeProviderExtensions%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/egil/TimeProviderExtensions/main)

# TimeProvider Extensions

Testing extensions for the [`System.TimeProvider`](https://learn.microsoft.com/en-us/dotnet/api/system.timeprovider) API. It includes:

- An advanced test/fake version of the `TimeProvider` type, named `ManualTimeProvider`, that allows you to control the progress of time during testing deterministically (see the difference to Microsoft's `FakeTimeProvider` below).
- A backported version of `PeriodicTimer` that supports `TimeProvider` in .NET 6.

## Quick start

This describes how to get started:

1. Get the latest release from https://www.nuget.org/packages/TimeProviderExtensions.

2. Take a dependency on `TimeProvider` in your production code. Inject the production version of `TimeProvider` available via the [`TimeProvider.System`](https://learn.microsoft.com/en-us/dotnet/api/system.timeprovider.system?#system-timeprovider-system) property during production.

3. During testing, inject the `ManualTimeProvider` from this library. This allows you to write tests that run fast and predictably.
   - Advance time by calling `Advance(TimeSpan)` or `SetUtcNow(DateTimeOffset)` or
   - Jump ahead in time using `Jump(TimeSpan)` or `Jump(DateTimeOffset)`.

4. See the **[`ManualTimeProvider API`](https://github.com/egil/TimeProviderExtensions/blob/main/docs/TimeProviderExtensions.ManualTimeProvider.md) page** for the full API documentation for `ManualTimeProvider`.

5. Read the rest of this README for further details and examples.

## API Overview

These pages have all the details of the API included in this package:

- [`ManualTimeProvider`](https://github.com/egil/TimeProviderExtensions/blob/main/docs/TimeProviderExtensions.ManualTimeProvider.md)
- [`AutoAdvanceBehavior`](https://github.com/egil/TimeProviderExtensions/blob/main/docs/TimeProviderExtensions.AutoAdvanceBehavior.md)
- [`ManualTimer`](https://github.com/egil/TimeProviderExtensions/blob/main/docs/TimeProviderExtensions.ManualTimer.md)

**.NET 7 and earlier:**

- [`PeriodicTimerWrapper`](https://github.com/egil/TimeProviderExtensions/blob/main/docs/System.Threading.PeriodicTimerWrapper.md)
- [`TimeProviderPeriodicTimerExtensions`](https://github.com/egil/TimeProviderExtensions/blob/main/docs/System.Threading.TimeProviderPeriodicTimerExtensions.md)

## Known limitations and issues:

- If running on .NET versions earlier than .NET 8.0, there is a constraint when invoking `CancellationTokenSource.CancelAfter(TimeSpan)` on the `CancellationTokenSource` object returned by `CreateCancellationTokenSource(TimeSpan delay)`. This action will not terminate the initial timer indicated by the `delay` argument initially passed the `CreateCancellationTokenSource` method. However, this restriction does not apply to .NET 8.0 and later versions.
- To enable controlling `PeriodicTimer` via `TimeProvider` in versions of .NET earlier than .NET 8.0, the `TimeProvider.CreatePeriodicTimer` returns a `PeriodicTimerWrapper` object instead of a `PeriodicTimer` object. The `PeriodicTimerWrapper` type is just a lightweight wrapper around the original `System.Threading.PeriodicTimer` and will behave identically to it.
- If `ManualTimeProvider` is created via [AutoFixture](https://github.com/AutoFixture/AutoFixture), be aware that will set writable properties with random values. This behavior can be overridden by providing a customization to AutoFixture, e.g.:
  ```c#
  fixture.Customize<ManualTimeProvider>(x => x.OmitAutoProperties()));
  ```
  or by using the `[NoAutoProperties]` attribute, if using `AutoFixture.Xunit2`.

## Installation and Usage

Get the latest release from https://www.nuget.org/packages/TimeProviderExtensions

### Set up in production

To use in production, pass in `TimeProvider.System` to the types that depend on `TimeProvider`.
This can be done directly or via an IoC Container, e.g., .NETs built-in `IServiceCollection` like so:

```c#
services.AddSingleton(TimeProvider.System);
```

If you do not want to register the `TimeProvider` with your IoC container, you can instead create
an additional constructor in the types that use it, which allows you to pass in a `TimeProvider`,
and in the existing constructor(s) you have, just new up `TimeProvider.System` directly. For example:

```c#
public class MyService
{
  private readonly TimeProvider timeProvider;

  public MyService() : this(TimeProvider.System)
  {
  }

  public MyService(TimeProvider timeProvider)
  {
    this.timeProvider = timeProvider;
  }
}
```

This allows you to explicitly pass in a `ManualTimeProvider` during testing.

### Example - control time during tests

If a system under test (SUT) uses things like `Task.Delay`, `DateTimeOffset.UtcNow`, `Task.WaitAsync`, or `PeriodicTimer`,
it becomes hard to create tests that run fast and predictably.

The idea is to replace the use of e.g. `Task.Delay` with an abstraction, the `TimeProvider`, that in production
is represented by the `TimeProvider.System`, which just uses the real `Task.Delay`. During testing it is now possible to
pass in `ManualTimeProvider`, which allows the test to control the progress of time, making it possible to skip ahead,
e.g. 10 minutes, and also pause time, leading to fast and predictable tests.

As an example, let us test the "Stuff Service" below that performs specific tasks every 10 seconds with an additional
1-second delay. We have two versions, one that uses the standard types in .NET, and one that uses the `TimeProvider`.

```c#
// Version of stuff service that uses the built-in DateTimeOffset, PeriodicTimer, and Task.Delay
public class StuffService
{
  private static readonly TimeSpan doStuffDelay = TimeSpan.FromSeconds(10);
  private readonly List<DateTimeOffset> container;

  public StuffService(List<DateTimeOffset> container)
  {
    this.container = container;
  }

  public async Task DoStuff(CancellationToken cancelllationToken)
  {
    using var periodicTimer = new PeriodicTimer(doStuffDelay);

    while (await periodicTimer.WaitForNextTickAsync(cancellationToken))
    {
      await Task.Delay(TimeSpan.FromSeconds(1));
      container.Add(DateTimeOffset.UtcNow);
    }
  }
}

// Version of stuff service that uses the built-in TimeProvider
public class StuffServiceUsingTimeProvider
{
  private static readonly TimeSpan doStuffDelay = TimeSpan.FromSeconds(10);
  private readonly TimeProvider timeProvider;
  private readonly List<DateTimeOffset> container;

  public StuffServiceUsingTimeProvider(TimeProvider timeProvider, List<DateTimeOffset> container)
  {
    this.timeProvider = timeProvider;
    this.container = container;
  }

  public async Task DoStuff(CancellationToken cancelllationToken)
  {
    using var periodicTimer = timeProvider.CreatePeriodicTimer(doStuffDelay);

    while (await periodicTimer.WaitForNextTickAsync(cancellationToken))
    {
      await timeProvider.Delay(TimeSpan.FromSeconds(1));
      container.Add(timeProvider.GetUtcNow());
    }
  }
}
```

The test, using xUnit and FluentAssertions, could look like this:

```c#
[Fact]
public void DoStuff_does_stuff_every_11_seconds()
{
  // Arrange
  var timeProvider = new ManualTimeProvider();
  var container = new List<DateTimeOffset>();
  var sut = new StuffServiceUsingTimeProvider(timeProvider, container);

  // Act
  _ = sut.DoStuff(CancellationToken.None);
  timeProvider.Advance(TimeSpan.FromSeconds(11));

  // Assert
  container
    .Should()
    .ContainSingle()
    .Which
    .Should()
    .Be(timeProvider.GetUtcNow());
}
```

This test will run in nanoseconds and is deterministic.

Compare that to the similar test below for `StuffService` that needs to wait for 11 seconds before it can safely assert that the expectation has been met.

```c#
[Fact]
public async Task DoStuff_does_stuff_every_11_seconds()
{
  // Arrange
  var container = new List<DateTimeOffset>();
  var sut = new StuffService(container);

  // Act
  _ = sut.DoStuff(CancellationToken.None);
  await Task.Delay(TimeSpan.FromSeconds(11));

  // Assert
  container
    .Should()
    .ContainSingle()
    .Which
    .Should()
    .BeCloseTo(DateTimeOffset.UtcNow, precision: TimeSpan.FromMilliseconds(50));
}
```

## Difference between `ManualTimeProvider` and `FakeTimeProvider`

The .NET team has published a similar test-specific time provider, the [`Microsoft.Extensions.Time.Testing.FakeTimeProvider`](https://www.nuget.org/packages/Microsoft.Extensions.Time.Testing.FakeTimeProvider/).

The public API of both `FakeTimeProvider` and `ManualTimeProvider` are compatible, but there are some differences in when time is set before timer callbacks. Let's illustrate this with an example:

For example, if we create an `ITimer` with a *due time* and *period* set to **1 second**, the `DateTimeOffset` returned from `GetUtcNow()` during the timer callback may be different depending on the amount passed to `Advance()` (or `SetUtcNow()`).

If we call `Advance(TimeSpan.FromSeconds(1))` three times, effectively moving time forward by three seconds, the timer callback will be invoked once at times `00:01`, `00:02`, and `00:03`, as illustrated in the drawing below. Both `FakeTimeProvider` and `ManualTimeProvider` behave like this:

![Advancing time by three seconds in one-second increments.](https://raw.githubusercontent.com/egil/TimeProviderExtensions/main/docs/advance-1-second.svg)

If we instead call `Advance(TimeSpan.FromSeconds(3))` once, the two implementations behave differently. `ManualTimeProvider` will invoke the timer callback at the same time (`00:01`, `00:02`, and `00:03`) as if we had called `Advance(TimeSpan.FromSeconds(1))` three times, as illustrated in the drawing below:

![Advancing time by three seconds in one step using ManualTimeProvider.](https://raw.githubusercontent.com/egil/TimeProviderExtensions/main/docs/ManualTimeProvider-advance-3-seconds.svg)

However, `FakeTimeProvider` will invoke the timer callback at time `00:03` three times, as illustrated in the drawings below:

![Advancing time by three seconds in one step using FakeTimeProvider.](https://raw.githubusercontent.com/egil/TimeProviderExtensions/main/docs/FakeTimeProvider-advance-3-seconds.svg)

Technically, both implementations are correct since the `ITimer` abstractions only promise to invoke the callback timer *on or after the due time/period has elapsed*, never before.

However, I strongly prefer the `ManualTimeProvider` approach since it behaves consistently independent of how time is moved forward. It seems much more in the spirit of how a deterministic time provider should behave and avoids users being surprised when writing tests. I imagine users may get stuck for a while trying to debug why the time reported by `GetUtcNow()` is not set as expected due to the subtle difference in the behavior of `FakeTimeProvider`.

That said, it can be useful to test that your code behaves correctly if a timer isn't allocated processor time immediately when it's callback should fire, and for that, `ManualTimeProvider` includes a different method, `Jump`.

### Jumping to a point in time

A real `ITimer`'s callback may not be allocated processor time and be able to fire at the moment it has been scheduled, e.g. if the processor is busy doing other things. The callback will eventually fire (unless the timer is disposed of).

To support testing this scenario, `ManualtTimeProvider` includes a method that will jump time to a specific point, and then invoke all scheduled timer callbacks between the start and end of the jump. This behavior is similar to how `FakeTimeProvider`s `Advance` method works, as described in the previous section.

![Jumping ahead in time by three seconds in one step using ManualTimeProvider.](https://raw.githubusercontent.com/egil/TimeProviderExtensions/main/docs/jump-3-seconds.svg)
