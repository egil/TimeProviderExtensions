#### [TimeProviderExtensions](index.md 'index')
### [TimeProviderExtensions](index.md#TimeProviderExtensions 'TimeProviderExtensions')

## ManualTimeProvider Class

Represents a synthetic time provider that can be used to enable deterministic behavior in tests.

```csharp
public class ManualTimeProvider : System.TimeProvider
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [System.TimeProvider](https://docs.microsoft.com/en-us/dotnet/api/System.TimeProvider 'System.TimeProvider') &#129106; ManualTimeProvider

### Remarks
Learn more at [https://github.com/egil/TimeProviderExtensions](https://github.com/egil/TimeProviderExtensions 'https://github.com/egil/TimeProviderExtensions').
### Constructors

<a name='TimeProviderExtensions.ManualTimeProvider.ManualTimeProvider()'></a>

## ManualTimeProvider() Constructor

Initializes a new instance of the [ManualTimeProvider](TimeProviderExtensions.ManualTimeProvider.md 'TimeProviderExtensions.ManualTimeProvider') class.

```csharp
public ManualTimeProvider();
```

### Remarks
This creates a provider whose time is initially set to midnight January 1st 2000 and
with the local time zone set to [System.TimeZoneInfo.Utc](https://docs.microsoft.com/en-us/dotnet/api/System.TimeZoneInfo.Utc 'System.TimeZoneInfo.Utc').
The provider is set to not automatically advance time each time it is read.

<a name='TimeProviderExtensions.ManualTimeProvider.ManualTimeProvider(System.DateTimeOffset)'></a>

## ManualTimeProvider(DateTimeOffset) Constructor

Initializes a new instance of the [ManualTimeProvider](TimeProviderExtensions.ManualTimeProvider.md 'TimeProviderExtensions.ManualTimeProvider') class.

```csharp
public ManualTimeProvider(System.DateTimeOffset startDateTime);
```
#### Parameters

<a name='TimeProviderExtensions.ManualTimeProvider.ManualTimeProvider(System.DateTimeOffset).startDateTime'></a>

`startDateTime` [System.DateTimeOffset](https://docs.microsoft.com/en-us/dotnet/api/System.DateTimeOffset 'System.DateTimeOffset')

The initial time and date reported by the provider.

### Remarks
The local time zone set to [System.TimeZoneInfo.Utc](https://docs.microsoft.com/en-us/dotnet/api/System.TimeZoneInfo.Utc 'System.TimeZoneInfo.Utc').
The provider is set to not automatically advance time each time it is read.

<a name='TimeProviderExtensions.ManualTimeProvider.ManualTimeProvider(System.DateTimeOffset,System.TimeZoneInfo)'></a>

## ManualTimeProvider(DateTimeOffset, TimeZoneInfo) Constructor

Initializes a new instance of the [ManualTimeProvider](TimeProviderExtensions.ManualTimeProvider.md 'TimeProviderExtensions.ManualTimeProvider') class.

```csharp
public ManualTimeProvider(System.DateTimeOffset startDateTime, System.TimeZoneInfo localTimeZone);
```
#### Parameters

<a name='TimeProviderExtensions.ManualTimeProvider.ManualTimeProvider(System.DateTimeOffset,System.TimeZoneInfo).startDateTime'></a>

`startDateTime` [System.DateTimeOffset](https://docs.microsoft.com/en-us/dotnet/api/System.DateTimeOffset 'System.DateTimeOffset')

The initial time and date reported by the provider.

<a name='TimeProviderExtensions.ManualTimeProvider.ManualTimeProvider(System.DateTimeOffset,System.TimeZoneInfo).localTimeZone'></a>

`localTimeZone` [System.TimeZoneInfo](https://docs.microsoft.com/en-us/dotnet/api/System.TimeZoneInfo 'System.TimeZoneInfo')

Optional local time zone to use during testing. Defaults to [System.TimeZoneInfo.Utc](https://docs.microsoft.com/en-us/dotnet/api/System.TimeZoneInfo.Utc 'System.TimeZoneInfo.Utc').

### Remarks
The provider is set to not automatically advance time each time it is read.
### Properties

<a name='TimeProviderExtensions.ManualTimeProvider.ActiveTimers'></a>

## ManualTimeProvider.ActiveTimers Property

Gets the number of active [ManualTimer](TimeProviderExtensions.ManualTimer.md 'TimeProviderExtensions.ManualTimer'), that have callbacks that are scheduled to be triggered at some point in the future.

```csharp
public int ActiveTimers { get; }
```

#### Property Value
[System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

<a name='TimeProviderExtensions.ManualTimeProvider.AutoAdvanceAmount'></a>

## ManualTimeProvider.AutoAdvanceAmount Property

Gets or sets the amount of time by which time advances whenever the clock is read via [GetUtcNow()](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.GetUtcNow() 'TimeProviderExtensions.ManualTimeProvider.GetUtcNow()').

```csharp
public System.TimeSpan AutoAdvanceAmount { get; set; }
```

#### Property Value
[System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan')

#### Exceptions

[System.ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/System.ArgumentOutOfRangeException 'System.ArgumentOutOfRangeException')  
Thrown when set to a value than [System.TimeSpan.Zero](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan.Zero 'System.TimeSpan.Zero').

### Remarks
Set to [System.TimeSpan.Zero](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan.Zero 'System.TimeSpan.Zero') to disable auto advance. The default value is [System.TimeSpan.Zero](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan.Zero 'System.TimeSpan.Zero').

<a name='TimeProviderExtensions.ManualTimeProvider.LocalTimeZone'></a>

## ManualTimeProvider.LocalTimeZone Property

Gets a [System.TimeZoneInfo](https://docs.microsoft.com/en-us/dotnet/api/System.TimeZoneInfo 'System.TimeZoneInfo') object that represents the local time zone according to this [System.TimeProvider](https://docs.microsoft.com/en-us/dotnet/api/System.TimeProvider 'System.TimeProvider')'s notion of time.

```csharp
public override System.TimeZoneInfo LocalTimeZone { get; }
```

#### Property Value
[System.TimeZoneInfo](https://docs.microsoft.com/en-us/dotnet/api/System.TimeZoneInfo 'System.TimeZoneInfo')

### Remarks
The default implementation returns [System.TimeZoneInfo.Local](https://docs.microsoft.com/en-us/dotnet/api/System.TimeZoneInfo.Local 'System.TimeZoneInfo.Local').

<a name='TimeProviderExtensions.ManualTimeProvider.Start'></a>

## ManualTimeProvider.Start Property

Gets the starting date and time for this provider.

```csharp
public System.DateTimeOffset Start { get; set; }
```

#### Property Value
[System.DateTimeOffset](https://docs.microsoft.com/en-us/dotnet/api/System.DateTimeOffset 'System.DateTimeOffset')

<a name='TimeProviderExtensions.ManualTimeProvider.TimestampFrequency'></a>

## ManualTimeProvider.TimestampFrequency Property

Gets the amount by which the value from [GetTimestamp()](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.GetTimestamp() 'TimeProviderExtensions.ManualTimeProvider.GetTimestamp()') increments per second.

```csharp
public override long TimestampFrequency { get; }
```

#### Property Value
[System.Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64 'System.Int64')

### Remarks
This is fixed to the value of [System.TimeSpan.TicksPerSecond](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan.TicksPerSecond 'System.TimeSpan.TicksPerSecond').
### Methods

<a name='TimeProviderExtensions.ManualTimeProvider.Advance(System.TimeSpan)'></a>

## ManualTimeProvider.Advance(TimeSpan) Method

Advances time by a specific amount.

```csharp
public void Advance(System.TimeSpan delta);
```
#### Parameters

<a name='TimeProviderExtensions.ManualTimeProvider.Advance(System.TimeSpan).delta'></a>

`delta` [System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan')

The amount of time to advance the clock by.

#### Exceptions

[System.ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/System.ArgumentOutOfRangeException 'System.ArgumentOutOfRangeException')  
Thrown if [delta](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.Advance(System.TimeSpan).delta 'TimeProviderExtensions.ManualTimeProvider.Advance(System.TimeSpan).delta') is negative. Going back in time is not supported.

### Remarks
Advancing time affects the timers created from this provider, and all other operations that are directly or
indirectly using this provider as a time source. Whereas when using [System.TimeProvider.System](https://docs.microsoft.com/en-us/dotnet/api/System.TimeProvider.System 'System.TimeProvider.System'), time
marches forward automatically in hardware, for the manual time provider the application is responsible for
doing this explicitly by calling this method.

If advancing time moves it paste multiple scheduled timer callbacks, the current
date/time reported by [GetUtcNow()](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.GetUtcNow() 'TimeProviderExtensions.ManualTimeProvider.GetUtcNow()') at the point when each callback is invoked will match the
due time of the callback.

For example:

```csharp
var start = sut.GetTimestamp();

var timer = manualTimeProvider.CreateTimer(
                callback: _ => manualTimeProvider.GetElapsedTime(start),
                state: null,
                dueTime: Span.FromSecond(1),
                period: TimeSpan.FromSecond(1));

manualtTimeProvider.Advance(TimeSpan.FromSecond(3));
```
The call to `Advance(TimeSpan.FromSecond(3))` causes the `timer`s callback to be invoked three times,
and the result of the `manualTimeProvider.GetElapsedTime(start)` in the callback call will be <em>1 second</em>, <em>2 seconds</em>,
and <em>3 seconds</em>. In other words, the time of the provider is set before the time callback is invoked
to the time that the callback is scheduled to be invoked at.

If the desired result is to jump time by [delta](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.Advance(System.TimeSpan).delta 'TimeProviderExtensions.ManualTimeProvider.Advance(System.TimeSpan).delta') and then invoke the timer callbacks
the expected number of times, i.e. such that the result of `manualTimeProvider.GetElapsedTime(start)` in the callback is
<em>3 seconds</em>, <em>3 seconds</em>, and <em>3 seconds</em>, use [Jump(DateTimeOffset)](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.Jump(System.DateTimeOffset) 'TimeProviderExtensions.ManualTimeProvider.Jump(System.DateTimeOffset)') or [Jump(TimeSpan)](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.Jump(System.TimeSpan) 'TimeProviderExtensions.ManualTimeProvider.Jump(System.TimeSpan)') instead.

Learn more about this behavior at <seealso href="https://github.com/egil/TimeProviderExtensions/#difference-between-manualtimeprovider-and-faketimeprovider"/>.

<a name='TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback,object,System.TimeSpan,System.TimeSpan)'></a>

## ManualTimeProvider.CreateTimer(TimerCallback, object, TimeSpan, TimeSpan) Method

Creates a new [System.Threading.ITimer](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.ITimer 'System.Threading.ITimer') instance, using [System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan') values to measure time intervals.

```csharp
public override System.Threading.ITimer CreateTimer(System.Threading.TimerCallback callback, object? state, System.TimeSpan dueTime, System.TimeSpan period);
```
#### Parameters

<a name='TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback,object,System.TimeSpan,System.TimeSpan).callback'></a>

`callback` [System.Threading.TimerCallback](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.TimerCallback 'System.Threading.TimerCallback')

A delegate representing a method to be executed when the timer fires. The method specified for callback should be reentrant,
as it may be invoked simultaneously on two threads if the timer fires again before or while a previous callback is still being handled.

<a name='TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback,object,System.TimeSpan,System.TimeSpan).state'></a>

`state` [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')

An object to be passed to the [callback](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback,object,System.TimeSpan,System.TimeSpan).callback 'TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback, object, System.TimeSpan, System.TimeSpan).callback'). This may be null.

<a name='TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback,object,System.TimeSpan,System.TimeSpan).dueTime'></a>

`dueTime` [System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan')

The amount of time to delay before [callback](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback,object,System.TimeSpan,System.TimeSpan).callback 'TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback, object, System.TimeSpan, System.TimeSpan).callback') is invoked. Specify [System.Threading.Timeout.InfiniteTimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Timeout.InfiniteTimeSpan 'System.Threading.Timeout.InfiniteTimeSpan') to prevent the timer from starting. Specify [System.TimeSpan.Zero](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan.Zero 'System.TimeSpan.Zero') to start the timer immediately.

<a name='TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback,object,System.TimeSpan,System.TimeSpan).period'></a>

`period` [System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan')

The time interval between invocations of [callback](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback,object,System.TimeSpan,System.TimeSpan).callback 'TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback, object, System.TimeSpan, System.TimeSpan).callback'). Specify [System.Threading.Timeout.InfiniteTimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Timeout.InfiniteTimeSpan 'System.Threading.Timeout.InfiniteTimeSpan') to disable periodic signaling.

#### Returns
[System.Threading.ITimer](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.ITimer 'System.Threading.ITimer')  
The newly created [System.Threading.ITimer](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.ITimer 'System.Threading.ITimer') instance.

#### Exceptions

[System.ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/System.ArgumentNullException 'System.ArgumentNullException')  
[callback](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback,object,System.TimeSpan,System.TimeSpan).callback 'TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback, object, System.TimeSpan, System.TimeSpan).callback') is null.

[System.ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/System.ArgumentOutOfRangeException 'System.ArgumentOutOfRangeException')  
The number of milliseconds in the value of [dueTime](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback,object,System.TimeSpan,System.TimeSpan).dueTime 'TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback, object, System.TimeSpan, System.TimeSpan).dueTime') or [period](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback,object,System.TimeSpan,System.TimeSpan).period 'TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback, object, System.TimeSpan, System.TimeSpan).period') is negative and not equal to [System.Threading.Timeout.Infinite](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Timeout.Infinite 'System.Threading.Timeout.Infinite'), or is greater than [System.Int32.MaxValue](https://docs.microsoft.com/en-us/dotnet/api/System.Int32.MaxValue 'System.Int32.MaxValue').

### Remarks

The delegate specified by the callback parameter is invoked once after [dueTime](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback,object,System.TimeSpan,System.TimeSpan).dueTime 'TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback, object, System.TimeSpan, System.TimeSpan).dueTime') elapses, and thereafter each time the [period](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback,object,System.TimeSpan,System.TimeSpan).period 'TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback, object, System.TimeSpan, System.TimeSpan).period') time interval elapses.

If [dueTime](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback,object,System.TimeSpan,System.TimeSpan).dueTime 'TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback, object, System.TimeSpan, System.TimeSpan).dueTime') is zero, the callback is invoked immediately. If [dueTime](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback,object,System.TimeSpan,System.TimeSpan).dueTime 'TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback, object, System.TimeSpan, System.TimeSpan).dueTime') is -1 milliseconds, [callback](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback,object,System.TimeSpan,System.TimeSpan).callback 'TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback, object, System.TimeSpan, System.TimeSpan).callback') is not invoked; the timer is disabled,
but can be re-enabled by calling the [System.Threading.ITimer.Change(System.TimeSpan,System.TimeSpan)](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.ITimer.Change#System_Threading_ITimer_Change_System_TimeSpan,System_TimeSpan_ 'System.Threading.ITimer.Change(System.TimeSpan,System.TimeSpan)') method.

If [period](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback,object,System.TimeSpan,System.TimeSpan).period 'TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback, object, System.TimeSpan, System.TimeSpan).period') is 0 or -1 milliseconds and [dueTime](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback,object,System.TimeSpan,System.TimeSpan).dueTime 'TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback, object, System.TimeSpan, System.TimeSpan).dueTime') is positive, [callback](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback,object,System.TimeSpan,System.TimeSpan).callback 'TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback, object, System.TimeSpan, System.TimeSpan).callback') is invoked once; the periodic behavior of the timer is disabled,
but can be re-enabled using the [System.Threading.ITimer.Change(System.TimeSpan,System.TimeSpan)](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.ITimer.Change#System_Threading_ITimer_Change_System_TimeSpan,System_TimeSpan_ 'System.Threading.ITimer.Change(System.TimeSpan,System.TimeSpan)') method.

The return [System.Threading.ITimer](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.ITimer 'System.Threading.ITimer') instance will be implicitly rooted while the timer is still scheduled.

[CreateTimer(TimerCallback, object, TimeSpan, TimeSpan)](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback,object,System.TimeSpan,System.TimeSpan) 'TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback, object, System.TimeSpan, System.TimeSpan)') captures the [System.Threading.ExecutionContext](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.ExecutionContext 'System.Threading.ExecutionContext') and stores that with the [System.Threading.ITimer](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.ITimer 'System.Threading.ITimer') for use in invoking [callback](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback,object,System.TimeSpan,System.TimeSpan).callback 'TimeProviderExtensions.ManualTimeProvider.CreateTimer(System.Threading.TimerCallback, object, System.TimeSpan, System.TimeSpan).callback')
            each time it's called. That capture can be suppressed with [System.Threading.ExecutionContext.SuppressFlow](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.ExecutionContext.SuppressFlow 'System.Threading.ExecutionContext.SuppressFlow').

To move time forward for the returned [System.Threading.ITimer](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.ITimer 'System.Threading.ITimer'), call [Advance(TimeSpan)](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.Advance(System.TimeSpan) 'TimeProviderExtensions.ManualTimeProvider.Advance(System.TimeSpan)') or [SetUtcNow(DateTimeOffset)](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.SetUtcNow(System.DateTimeOffset) 'TimeProviderExtensions.ManualTimeProvider.SetUtcNow(System.DateTimeOffset)') on this time provider.

<a name='TimeProviderExtensions.ManualTimeProvider.GetTimestamp()'></a>

## ManualTimeProvider.GetTimestamp() Method

Gets the current high-frequency value designed to measure small time intervals with high accuracy in the timer mechanism.

```csharp
public override long GetTimestamp();
```

#### Returns
[System.Int64](https://docs.microsoft.com/en-us/dotnet/api/System.Int64 'System.Int64')  
A long integer representing the high-frequency counter value of the underlying timer mechanism.

### Remarks
This implementation bases timestamp on [System.DateTimeOffset.UtcTicks](https://docs.microsoft.com/en-us/dotnet/api/System.DateTimeOffset.UtcTicks 'System.DateTimeOffset.UtcTicks'),
since the progression of time is represented by the date and time returned from [GetUtcNow()](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.GetUtcNow() 'TimeProviderExtensions.ManualTimeProvider.GetUtcNow()').

<a name='TimeProviderExtensions.ManualTimeProvider.GetUtcNow()'></a>

## ManualTimeProvider.GetUtcNow() Method

Gets a [System.DateTimeOffset](https://docs.microsoft.com/en-us/dotnet/api/System.DateTimeOffset 'System.DateTimeOffset') value whose date and time are set to the current
Coordinated Universal Time (UTC) date and time and whose offset is Zero,
all according to this [ManualTimeProvider](TimeProviderExtensions.ManualTimeProvider.md 'TimeProviderExtensions.ManualTimeProvider')'s notion of time.

```csharp
public override System.DateTimeOffset GetUtcNow();
```

#### Returns
[System.DateTimeOffset](https://docs.microsoft.com/en-us/dotnet/api/System.DateTimeOffset 'System.DateTimeOffset')

### Remarks
If [AutoAdvanceAmount](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.AutoAdvanceAmount 'TimeProviderExtensions.ManualTimeProvider.AutoAdvanceAmount') is greater than [System.TimeSpan.Zero](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan.Zero 'System.TimeSpan.Zero'), calling this
method will move time forward by the amount specified by [AutoAdvanceAmount](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.AutoAdvanceAmount 'TimeProviderExtensions.ManualTimeProvider.AutoAdvanceAmount').
The [System.DateTimeOffset](https://docs.microsoft.com/en-us/dotnet/api/System.DateTimeOffset 'System.DateTimeOffset') returned from this method will reflect the time before
the auto advance was applied, if any.

<a name='TimeProviderExtensions.ManualTimeProvider.Jump(System.DateTimeOffset)'></a>

## ManualTimeProvider.Jump(DateTimeOffset) Method

Jumps the date and time returned by [GetUtcNow()](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.GetUtcNow() 'TimeProviderExtensions.ManualTimeProvider.GetUtcNow()') to [value](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.Jump(System.DateTimeOffset).value 'TimeProviderExtensions.ManualTimeProvider.Jump(System.DateTimeOffset).value') and triggers any
scheduled items that are waiting for time to be forwarded.

```csharp
public void Jump(System.DateTimeOffset value);
```
#### Parameters

<a name='TimeProviderExtensions.ManualTimeProvider.Jump(System.DateTimeOffset).value'></a>

`value` [System.DateTimeOffset](https://docs.microsoft.com/en-us/dotnet/api/System.DateTimeOffset 'System.DateTimeOffset')

The new UtcNow time.

#### Exceptions

[System.ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/System.ArgumentOutOfRangeException 'System.ArgumentOutOfRangeException')  
Thrown if [value](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.Jump(System.DateTimeOffset).value 'TimeProviderExtensions.ManualTimeProvider.Jump(System.DateTimeOffset).value') is less than the value returned by [GetUtcNow()](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.GetUtcNow() 'TimeProviderExtensions.ManualTimeProvider.GetUtcNow()'). Going back in time is not supported.

### Remarks
Jumping time affects the timers created from this provider, and all other operations that are directly or
indirectly using this provider as a time source. Whereas when using [System.TimeProvider.System](https://docs.microsoft.com/en-us/dotnet/api/System.TimeProvider.System 'System.TimeProvider.System'), time
marches forward automatically in hardware, for the manual time provider the application is responsible for
doing this explicitly by calling this method.

If jumping time moves it paste one or more scheduled timer callbacks, the current
date/time reported by [GetUtcNow()](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.GetUtcNow() 'TimeProviderExtensions.ManualTimeProvider.GetUtcNow()') and [GetTimestamp()](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.GetTimestamp() 'TimeProviderExtensions.ManualTimeProvider.GetTimestamp()') will match the new date/time
based on the [value](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.Jump(System.DateTimeOffset).value 'TimeProviderExtensions.ManualTimeProvider.Jump(System.DateTimeOffset).value') specified in the request.

For example:

```csharp
var start = sut.GetTimestamp();

var timer = manualTimeProvider.CreateTimer(
                callback: _ => manualTimeProvider.GetElapsedTime(start),
                state: null,
                dueTime: Span.FromSecond(1),
                period: TimeSpan.FromSecond(1));

manualtTimeProvider.Jump(manualtTimeProvider.Start + TimeSpan.FromSecond(3));
```
The call to `Jump(manualtTimeProvider.Start + TimeSpan.FromSecond(3))` causes the `timer`s callback to be invoked three times,
and the result of the `manualTimeProvider.GetElapsedTime(start)` in the callback call will be <em>3 seconds</em>
during all three invocations.

If the desired result is that timer callbacks happens exactly at their scheduled callback time, i.e. such that the result
of `manualTimeProvider.GetElapsedTime(start)` in the callback will be <em>1 second</em>, <em>2 seconds</em>, and <em>3 seconds</em>,
use [Advance(TimeSpan)](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.Advance(System.TimeSpan) 'TimeProviderExtensions.ManualTimeProvider.Advance(System.TimeSpan)') or [SetUtcNow(DateTimeOffset)](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.SetUtcNow(System.DateTimeOffset) 'TimeProviderExtensions.ManualTimeProvider.SetUtcNow(System.DateTimeOffset)') instead.

Learn more about this behavior at <seealso href="https://github.com/egil/TimeProviderExtensions/#difference-between-manualtimeprovider-and-faketimeprovider"/>.

<a name='TimeProviderExtensions.ManualTimeProvider.Jump(System.TimeSpan)'></a>

## ManualTimeProvider.Jump(TimeSpan) Method

Jumps time by a specific amount.

```csharp
public void Jump(System.TimeSpan delta);
```
#### Parameters

<a name='TimeProviderExtensions.ManualTimeProvider.Jump(System.TimeSpan).delta'></a>

`delta` [System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan')

The amount of time to jump the clock by.

#### Exceptions

[System.ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/System.ArgumentOutOfRangeException 'System.ArgumentOutOfRangeException')  
Thrown if [delta](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.Jump(System.TimeSpan).delta 'TimeProviderExtensions.ManualTimeProvider.Jump(System.TimeSpan).delta') is negative. Going back in time is not supported.

### Remarks
Jumping time affects the timers created from this provider, and all other operations that are directly or
indirectly using this provider as a time source. Whereas when using [System.TimeProvider.System](https://docs.microsoft.com/en-us/dotnet/api/System.TimeProvider.System 'System.TimeProvider.System'), time
marches forward automatically in hardware, for the manual time provider the application is responsible for
doing this explicitly by calling this method.

If jumping time moves it paste one or more scheduled timer callbacks, the current
date/time reported by [GetUtcNow()](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.GetUtcNow() 'TimeProviderExtensions.ManualTimeProvider.GetUtcNow()') and [GetTimestamp()](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.GetTimestamp() 'TimeProviderExtensions.ManualTimeProvider.GetTimestamp()') will match the new date/time
based on the [delta](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.Jump(System.TimeSpan).delta 'TimeProviderExtensions.ManualTimeProvider.Jump(System.TimeSpan).delta') specified in the request.

For example:

```csharp
var start = sut.GetTimestamp();

var timer = manualTimeProvider.CreateTimer(
                callback: _ => manualTimeProvider.GetElapsedTime(start),
                state: null,
                dueTime: Span.FromSecond(1),
                period: TimeSpan.FromSecond(1));

manualtTimeProvider.Jump(TimeSpan.FromSecond(3));
```
The call to `Jump(TimeSpan.FromSecond(3))` causes the `timer`s callback to be invoked three times,
and the result of the `manualTimeProvider.GetElapsedTime(start)` in the callback call will be <em>3 seconds</em>
during all three invocations.

If the desired result is that timer callbacks happens exactly at their scheduled callback time, i.e. such that the result
of `manualTimeProvider.GetElapsedTime(start)` in the callback will be <em>1 second</em>, <em>2 seconds</em>, and <em>3 seconds</em>,
use [Advance(TimeSpan)](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.Advance(System.TimeSpan) 'TimeProviderExtensions.ManualTimeProvider.Advance(System.TimeSpan)') or [SetUtcNow(DateTimeOffset)](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.SetUtcNow(System.DateTimeOffset) 'TimeProviderExtensions.ManualTimeProvider.SetUtcNow(System.DateTimeOffset)') instead.

Learn more about this behavior at <seealso href="https://github.com/egil/TimeProviderExtensions/#difference-between-manualtimeprovider-and-faketimeprovider"/>.

<a name='TimeProviderExtensions.ManualTimeProvider.SetLocalTimeZone(System.TimeZoneInfo)'></a>

## ManualTimeProvider.SetLocalTimeZone(TimeZoneInfo) Method

Sets the local time zone.

```csharp
public void SetLocalTimeZone(System.TimeZoneInfo localTimeZone);
```
#### Parameters

<a name='TimeProviderExtensions.ManualTimeProvider.SetLocalTimeZone(System.TimeZoneInfo).localTimeZone'></a>

`localTimeZone` [System.TimeZoneInfo](https://docs.microsoft.com/en-us/dotnet/api/System.TimeZoneInfo 'System.TimeZoneInfo')

The local time zone.

<a name='TimeProviderExtensions.ManualTimeProvider.SetUtcNow(System.DateTimeOffset)'></a>

## ManualTimeProvider.SetUtcNow(DateTimeOffset) Method

Sets the date and time returned by [GetUtcNow()](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.GetUtcNow() 'TimeProviderExtensions.ManualTimeProvider.GetUtcNow()') to [value](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.SetUtcNow(System.DateTimeOffset).value 'TimeProviderExtensions.ManualTimeProvider.SetUtcNow(System.DateTimeOffset).value') and triggers any
scheduled items that are waiting for time to be forwarded.

```csharp
public void SetUtcNow(System.DateTimeOffset value);
```
#### Parameters

<a name='TimeProviderExtensions.ManualTimeProvider.SetUtcNow(System.DateTimeOffset).value'></a>

`value` [System.DateTimeOffset](https://docs.microsoft.com/en-us/dotnet/api/System.DateTimeOffset 'System.DateTimeOffset')

The new UtcNow time.

#### Exceptions

[System.ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/System.ArgumentOutOfRangeException 'System.ArgumentOutOfRangeException')  
Thrown if [value](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.SetUtcNow(System.DateTimeOffset).value 'TimeProviderExtensions.ManualTimeProvider.SetUtcNow(System.DateTimeOffset).value') is less than the value returned by [GetUtcNow()](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.GetUtcNow() 'TimeProviderExtensions.ManualTimeProvider.GetUtcNow()'). Going back in time is not supported.

### Remarks
Setting time affects the timers created from this provider, and all other operations that are directly or
indirectly using this provider as a time source. Whereas when using [System.TimeProvider.System](https://docs.microsoft.com/en-us/dotnet/api/System.TimeProvider.System 'System.TimeProvider.System'), time
marches forward automatically in hardware, for the manual time provider the application is responsible for
doing this explicitly by calling this method.

If the set time moves it paste multiple scheduled timer callbacks, the current
date/time reported by [GetUtcNow()](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.GetUtcNow() 'TimeProviderExtensions.ManualTimeProvider.GetUtcNow()') at the point when each callback is invoked will match the
due time of the callback.

For example:

```csharp
var start = sut.GetTimestamp();

var timer = manualTimeProvider.CreateTimer(
                callback: _ => manualTimeProvider.GetElapsedTime(start),
                state: null,
                dueTime: Span.FromSecond(1),
                period: TimeSpan.FromSecond(1));

manualtTimeProvider.SetUtcNow(manualtTimeProvider.Start + TimeSpan.FromSecond(3));
```
The call to `SetUtcNow(manualtTimeProvider.Start + TimeSpan.FromSecond(3))` causes the `timer`s callback to be invoked three times,
and the result of the `manualTimeProvider.GetElapsedTime(start)` in the callback call will be <em>1 second</em>, <em>2 seconds</em>,
and <em>3 seconds</em>. In other words, the time of the provider is set before the time callback is invoked
to the time that the callback is scheduled to be invoked at.

If the desired result is to jump to the time specified in [value](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.SetUtcNow(System.DateTimeOffset).value 'TimeProviderExtensions.ManualTimeProvider.SetUtcNow(System.DateTimeOffset).value') and then invoke the timer callbacks
the expected number of times, i.e. such that the result of `manualTimeProvider.GetElapsedTime(start)` in the callback is
<em>3 seconds</em>, <em>3 seconds</em>, and <em>3 seconds</em>, use [Jump(DateTimeOffset)](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.Jump(System.DateTimeOffset) 'TimeProviderExtensions.ManualTimeProvider.Jump(System.DateTimeOffset)') or [Jump(TimeSpan)](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.Jump(System.TimeSpan) 'TimeProviderExtensions.ManualTimeProvider.Jump(System.TimeSpan)') instead.

Learn more about this behavior at <seealso href="https://github.com/egil/TimeProviderExtensions/#difference-between-manualtimeprovider-and-faketimeprovider"/>.

<a name='TimeProviderExtensions.ManualTimeProvider.ToString()'></a>

## ManualTimeProvider.ToString() Method

Returns a string representation this clock's current time.

```csharp
public override string ToString();
```

#### Returns
[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
A string representing the clock's current time.