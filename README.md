# Scheduler
A library that wraps common .NET scheduling and time related operations in an abstraction, that enables controlling time during testing.

## Control time during tests

If a system under test (SUT) uses things like `Task.Delay`, `DateTimeOffset.UtcNow`, or `PeriodicTimer`, 
it becomes hard to create tests that runs fast and predictably.

The idea is to replace the use of e.g. `Task.Delay` with an abstraction, the `ITimeScheduler`, that in production
is represented by the `TimeScheduler`, that just uses the real `Task.Delay`. During testing it is now possible to
pass in `TestScheduler`, that allows the test to control the progress of time, making it possible to skip ahead,
e.g. 10 minutes, and also pause time, leading to fast and predictable tests.

As an example, lets test the `StuffService` below that performs a specific tasks every 10 second:

```c#
public class StuffService 
{
  private static readonly TimeSpan doStuffDelay = TimeSpan.FromSeconds(10);
  private readonly ITimeScheduler scheduler;
  private readonly List<string> container;

  public StuffService(ITimeScheduler scheduler, List<string> container)
  {
    this.scheduler = scheduler;
    this.container = container;
  }
  
  public async Task DoStuff(CancellationToken cancelllationToken)
  {
    using var periodicTimer = scheduler.PeriodicTimer(doStuffDelay);
    
    while (await periodicTimer.WaitForNextTickAsync(cancellationToken))
    {
      container.Add("stuff");    
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
  var container = new List<string>();  
  var sut = new StuffService(scheduler, container);
  
  // Act
  _ = sut.DoStuff(CancellationToken.None);
  scheduler.ForwardTime(TimeSpan.FromSeconds(10));
  
  // Assert
  container.Should().ContainSingle();
}
```
