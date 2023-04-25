# Time Scheduler - a TimeProvider shim

This is a shim for the upcoming `System.TimeProvider` API coming in .NET 8. It includes a test version of the `TimeProvider`, named `ManualTimeProvider`, that allows you to control the progress of time during testing deterministically.

*NOTE: Originally, this library provided its own abstraction, `ITimeScheduler` and related types, `DefaultScheduler` and `TestScheduler`. These are now considered obsolete.*

Currently, the following .NET time-based APIs are supported:

| TimeProvider method | .NET API it replaces |
|----------------------|----------------------|
| `GetUtcNow()` method | `DateTimeOffset.UtcNow` property |
| `CreateTimer()` method | `System.Threading.Timer` type |
| `CreatePeriodicTimer(TimeSpan)` method | `System.Threading.PeriodicTimer` type |
| `Delay(TimeSpan, CancellationToken)` method | `Task.Delay(TimeSpan, CancellationToken)` method |
| `CancellationTokenSource.CancelAfter(TimeSpan, TimeProvider)` method | `CancellationTokenSource.CancelAfter(TimeSpan)` method |
| `Task.WaitAsync(TimeSpan, TimeProvider)` method | `Task.WaitAsync(TimeSpan)` method |
| `Task.WaitAsync(TimeSpan, TimeProvider, CancellationToken)` method | `Task.WaitAsync(TimeSpan, CancellationToken)` method |

The implementations of `TimeProvider` is abstract. An instance of `TimeProvider` for production use is availalbe on the `TimeProvider.System` property,
and `ManualTimeProvider` can be used during testing.

During testing, you can move time forward by calling `ForwardTime(TimeSpan)` or `SetUtcNow(DateTimeOffset)` on `ManualTimeProvider`. This allows
you to write tests that run fast and predictable, even if the system under test pauses execution for
multiple minutes using e.g. `TimeProvider.Delay(TimeSpan)`, the replacement for `Task.Delay(TimeSpan)`.

## Installation

Get the latest release from https://www.nuget.org/packages/TimeScheduler

## Set up in production

To use in production, pass in `TimeProvider.System` to the types that depend on `TimeProvider`. 
This can be done directly, or via an IoC Container, e.g. .NETs built-in `IServiceCollection` like so:

```c#
services.AddSingleton(TimeProvider.System);
```

If you do not want to register the `TimeProvider` with your IoC container, you can instead create
an additional constructor in the types that use it, which allow you to pass in a `TimeProvider`,
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

This allows you to explicitly pass in an `ManualTimeProvider` during testing.

## Example - control time during tests

If a system under test (SUT) uses things like `Task.Delay`, `DateTimeOffset.UtcNow`, `Task.WaitAsync`, or `PeriodicTimer`, 
it becomes hard to create tests that runs fast and predictably.

The idea is to replace the use of e.g. `Task.Delay` with an abstraction, the `TimeProvider`, that in production
is represented by the `TimeProvider.System`, that just uses the real `Task.Delay`. During testing it is now possible to
pass in `ManualTimeProvider`, that allows the test to control the progress of time, making it possible to skip ahead,
e.g. 10 minutes, and also pause time, leading to fast and predictable tests.

As an example, lets test the "Stuff Service" below that performs a specific tasks every 10 second with an additional 
1 second delay. We have two versions, one that uses the standard types in .NET, and one that uses the `TimeProvider`.

```c#
// Version of stuff service that uses the built in DateTimeOffset, PeriodicTimer, and Task.Delay
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

// Version of stuff service that uses the built in TimeProvider
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
  timeProvider.ForwardTime(TimeSpan.FromSeconds(11));
  
  // Assert
  container
    .Should()
    .ContainSingle()
    .Which
    .Should()
    .Be(timeProvider.GetUtcNow());
}
```

This test will run in nanoseconds, and is deterministic.

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
