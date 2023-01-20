# Time Scheduler
A library that wraps common .NET scheduling and time related operations in an abstraction, that enables controlling time during testing.

## Installation

Get the latest release from https://www.nuget.org/packages/TimeScheduler

## Control time during tests

If a system under test (SUT) uses things like `Task.Delay`, `DateTimeOffset.UtcNow`, or `PeriodicTimer`, 
it becomes hard to create tests that runs fast and predictably.

The idea is to replace the use of e.g. `Task.Delay` with an abstraction, the `ITimeScheduler`, that in production
is represented by the `DefaultScheduler`, that just uses the real `Task.Delay`. During testing it is now possible to
pass in `TestScheduler`, that allows the test to control the progress of time, making it possible to skip ahead,
e.g. 10 minutes, and also pause time, leading to fast and predictable tests.

As an example, lets test the "Stuff Service" below that performs a specific tasks every 10 second with an additional 
1 second delay. We have two versions, one that uses the standard types in .NET, and one that uses the `ITimeScheduler`.

```c#
// Version of stuff service that uses the built in DateTimeOffset, PeriodicTimer, and Task.Delay
public class StuffServiceSystem
{
  private static readonly TimeSpan doStuffDelay = TimeSpan.FromSeconds(10);
  private readonly List<DateTimeOffset> container;

  public StuffServiceSystem(List<DateTimeOffset> container)
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

// Version of stuff service that uses the built in TimeScheduler
public class StuffServiceUsingTimeScheduler 
{
  private static readonly TimeSpan doStuffDelay = TimeSpan.FromSeconds(10);
  private readonly ITimeScheduler scheduler;
  private readonly List<DateTimeOffset> container;

  public StuffServiceUsingTimeScheduler(ITimeScheduler scheduler, List<DateTimeOffset> container)
  {
    this.scheduler = scheduler;
    this.container = container;
  }
  
  public async Task DoStuff(CancellationToken cancelllationToken)
  {
    using var periodicTimer = scheduler.PeriodicTimer(doStuffDelay);
    
    while (await periodicTimer.WaitForNextTickAsync(cancellationToken))
    {      
      await scheduler.Delay(TimeSpan.FromSeconds(1));
      container.Add(scheduler.UtcNow);
    }
  }
}
```

The test, using xUnit and FluentAssertions, could look like this:

```c#
[Fact]
public void DoStuff_does_stuff_every_10_seconds()
{
  // Arrange
  var scheduler = new TestScheduler();
  var container = new List<DateTimeOffset>();  
  var sut = new StuffServiceUsingTimeScheduler(scheduler, container);
  
  // Act
  _ = sut.DoStuff(CancellationToken.None);
  scheduler.ForwardTime(TimeSpan.FromSeconds(11));
  
  // Assert
  container
    .Should()
    .ContainSingle()
    .Which
    .Should()
    .Be(scheduler.UtcNow);
}
```

Writing a similar test for `StuffServiceSystem` is both more simple and runs much slower.

```c#
[Fact]
public async Task DoStuff_does_stuff_every_10_seconds()
{
  // Arrange
  var container = new List<DateTimeOffset>();  
  var sut = new StuffServiceSystem(container);
  
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

## Set up in production

To use in production, pass in `DefaultScheduler` to the types that depend on `ITimeScheduler`. 
This can be done directly, or via an IoC Container, e.g. .NETs built-in `IServiceCollection` like so:

```c#
services.AddSingleton<ITimeScheduler, DefaultScheduler>();
```
