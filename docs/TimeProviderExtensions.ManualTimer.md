#### [TimeProviderExtensions](index.md 'index')
### [TimeProviderExtensions](index.md#TimeProviderExtensions 'TimeProviderExtensions')

## ManualTimer Class

A implementation of a [System.Threading.ITimer](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.ITimer 'System.Threading.ITimer') whose callbacks are scheduled via a [ManualTimeProvider](TimeProviderExtensions.ManualTimeProvider.md 'TimeProviderExtensions.ManualTimeProvider').

```csharp
public class ManualTimer :
System.IDisposable,
System.IAsyncDisposable
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [System.Threading.ITimer](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.ITimer 'System.Threading.ITimer') &#129106; ManualTimer

Implements [System.IDisposable](https://docs.microsoft.com/en-us/dotnet/api/System.IDisposable 'System.IDisposable'), [System.IAsyncDisposable](https://docs.microsoft.com/en-us/dotnet/api/System.IAsyncDisposable 'System.IAsyncDisposable')
### Constructors

<a name='TimeProviderExtensions.ManualTimer.ManualTimer(System.Threading.TimerCallback,object,TimeProviderExtensions.ManualTimeProvider)'></a>

## ManualTimer(TimerCallback, object, ManualTimeProvider) Constructor

Creates an instance of the [ManualTimer](TimeProviderExtensions.ManualTimer.md 'TimeProviderExtensions.ManualTimer'). No callbacks are scheduled during construction. Call [Change(TimeSpan, TimeSpan)](TimeProviderExtensions.ManualTimer.md#TimeProviderExtensions.ManualTimer.Change(System.TimeSpan,System.TimeSpan) 'TimeProviderExtensions.ManualTimer.Change(System.TimeSpan, System.TimeSpan)') to schedule invocations of [callback](TimeProviderExtensions.ManualTimer.md#TimeProviderExtensions.ManualTimer.ManualTimer(System.Threading.TimerCallback,object,TimeProviderExtensions.ManualTimeProvider).callback 'TimeProviderExtensions.ManualTimer.ManualTimer(System.Threading.TimerCallback, object, TimeProviderExtensions.ManualTimeProvider).callback') using the provided [timeProvider](TimeProviderExtensions.ManualTimer.md#TimeProviderExtensions.ManualTimer.ManualTimer(System.Threading.TimerCallback,object,TimeProviderExtensions.ManualTimeProvider).timeProvider 'TimeProviderExtensions.ManualTimer.ManualTimer(System.Threading.TimerCallback, object, TimeProviderExtensions.ManualTimeProvider).timeProvider').

```csharp
protected internal ManualTimer(System.Threading.TimerCallback callback, object? state, TimeProviderExtensions.ManualTimeProvider timeProvider);
```
#### Parameters

<a name='TimeProviderExtensions.ManualTimer.ManualTimer(System.Threading.TimerCallback,object,TimeProviderExtensions.ManualTimeProvider).callback'></a>

`callback` [System.Threading.TimerCallback](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.TimerCallback 'System.Threading.TimerCallback')

A delegate representing a method to be executed when the timer fires. The method specified for callback should be reentrant,
as it may be invoked simultaneously on two threads if the timer fires again before or while a previous callback is still being handled.

<a name='TimeProviderExtensions.ManualTimer.ManualTimer(System.Threading.TimerCallback,object,TimeProviderExtensions.ManualTimeProvider).state'></a>

`state` [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object')

An object to be passed to the [callback](TimeProviderExtensions.ManualTimer.md#TimeProviderExtensions.ManualTimer.ManualTimer(System.Threading.TimerCallback,object,TimeProviderExtensions.ManualTimeProvider).callback 'TimeProviderExtensions.ManualTimer.ManualTimer(System.Threading.TimerCallback, object, TimeProviderExtensions.ManualTimeProvider).callback'). This may be null.

<a name='TimeProviderExtensions.ManualTimer.ManualTimer(System.Threading.TimerCallback,object,TimeProviderExtensions.ManualTimeProvider).timeProvider'></a>

`timeProvider` [ManualTimeProvider](TimeProviderExtensions.ManualTimeProvider.md 'TimeProviderExtensions.ManualTimeProvider')

The [ManualTimeProvider](TimeProviderExtensions.ManualTimeProvider.md 'TimeProviderExtensions.ManualTimeProvider') which is used to schedule invocations of the [callback](TimeProviderExtensions.ManualTimer.md#TimeProviderExtensions.ManualTimer.ManualTimer(System.Threading.TimerCallback,object,TimeProviderExtensions.ManualTimeProvider).callback 'TimeProviderExtensions.ManualTimer.ManualTimer(System.Threading.TimerCallback, object, TimeProviderExtensions.ManualTimeProvider).callback') with.
### Properties

<a name='TimeProviderExtensions.ManualTimer.CallbackInvokeCount'></a>

## ManualTimer.CallbackInvokeCount Property

Gets the number of times a timer's callback has been invoked.

```csharp
public int CallbackInvokeCount { get; }
```

#### Property Value
[System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

<a name='TimeProviderExtensions.ManualTimer.CallbackTime'></a>

## ManualTimer.CallbackTime Property

Gets the next time the timer callback will be invoked, or [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null') if the timer is inactive.

```csharp
public System.Nullable<System.DateTimeOffset> CallbackTime { get; }
```

#### Property Value
[System.Nullable&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Nullable-1 'System.Nullable`1')[System.DateTimeOffset](https://docs.microsoft.com/en-us/dotnet/api/System.DateTimeOffset 'System.DateTimeOffset')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Nullable-1 'System.Nullable`1')

<a name='TimeProviderExtensions.ManualTimer.DueTime'></a>

## ManualTimer.DueTime Property

Gets the [System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan') representing the amount of time to delay before invoking the callback method specified when the [System.Threading.ITimer](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.ITimer 'System.Threading.ITimer') was constructed.

```csharp
public System.TimeSpan DueTime { get; set; }
```

#### Property Value
[System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan')

<a name='TimeProviderExtensions.ManualTimer.IsActive'></a>

## ManualTimer.IsActive Property

Gets whether the timer is currently active, i.e. has a future callback invocation scheduled.

```csharp
public bool IsActive { get; }
```

#### Property Value
[System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')

### Remarks
When [IsActive](TimeProviderExtensions.ManualTimer.md#TimeProviderExtensions.ManualTimer.IsActive 'TimeProviderExtensions.ManualTimer.IsActive') returns [true](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool'), [CallbackTime](TimeProviderExtensions.ManualTimer.md#TimeProviderExtensions.ManualTimer.CallbackTime 'TimeProviderExtensions.ManualTimer.CallbackTime') is not [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null').

<a name='TimeProviderExtensions.ManualTimer.Period'></a>

## ManualTimer.Period Property

Gets the time interval between invocations of the callback method specified when the Timer was constructed.
If set to [System.Threading.Timeout.InfiniteTimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Timeout.InfiniteTimeSpan 'System.Threading.Timeout.InfiniteTimeSpan') periodic signaling is disabled.

```csharp
public System.TimeSpan Period { get; set; }
```

#### Property Value
[System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan')
### Methods

<a name='TimeProviderExtensions.ManualTimer.~ManualTimer()'></a>

## ManualTimer.~ManualTimer() Method

The finalizer exists in case the timer is not disposed explicitly by the user.

```csharp
~ManualTimer();
```

<a name='TimeProviderExtensions.ManualTimer.Change(System.TimeSpan,System.TimeSpan)'></a>

## ManualTimer.Change(TimeSpan, TimeSpan) Method

Changes the start time and the interval between method invocations for a timer, using [System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan') values to measure time intervals.

```csharp
public virtual bool Change(System.TimeSpan dueTime, System.TimeSpan period);
```
#### Parameters

<a name='TimeProviderExtensions.ManualTimer.Change(System.TimeSpan,System.TimeSpan).dueTime'></a>

`dueTime` [System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan')

A [System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan') representing the amount of time to delay before invoking the callback method specified when the [System.Threading.ITimer](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.ITimer 'System.Threading.ITimer') was constructed.
Specify [System.Threading.Timeout.InfiniteTimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Timeout.InfiniteTimeSpan 'System.Threading.Timeout.InfiniteTimeSpan') to prevent the timer from restarting. Specify [System.TimeSpan.Zero](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan.Zero 'System.TimeSpan.Zero') to restart the timer immediately.

<a name='TimeProviderExtensions.ManualTimer.Change(System.TimeSpan,System.TimeSpan).period'></a>

`period` [System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan')

The time interval between invocations of the callback method specified when the Timer was constructed.
Specify [System.Threading.Timeout.InfiniteTimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Timeout.InfiniteTimeSpan 'System.Threading.Timeout.InfiniteTimeSpan') to disable periodic signaling.

#### Returns
[System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')  
[true](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool') if the timer was successfully updated; otherwise, [false](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool').

#### Exceptions

[System.ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/System.ArgumentOutOfRangeException 'System.ArgumentOutOfRangeException')  
The [dueTime](TimeProviderExtensions.ManualTimer.md#TimeProviderExtensions.ManualTimer.Change(System.TimeSpan,System.TimeSpan).dueTime 'TimeProviderExtensions.ManualTimer.Change(System.TimeSpan, System.TimeSpan).dueTime') or [period](TimeProviderExtensions.ManualTimer.md#TimeProviderExtensions.ManualTimer.Change(System.TimeSpan,System.TimeSpan).period 'TimeProviderExtensions.ManualTimer.Change(System.TimeSpan, System.TimeSpan).period') parameter, in milliseconds, is less than -1 or greater than 4294967294.

### Remarks
It is the responsibility of the implementer of the ITimer interface to ensure thread safety.

<a name='TimeProviderExtensions.ManualTimer.Dispose()'></a>

## ManualTimer.Dispose() Method

Disposes of the [ManualTimer](TimeProviderExtensions.ManualTimer.md 'TimeProviderExtensions.ManualTimer') and removes any scheduled callbacks from the [ManualTimeProvider](TimeProviderExtensions.ManualTimeProvider.md 'TimeProviderExtensions.ManualTimeProvider').

```csharp
public void Dispose();
```

Implements [Dispose()](https://docs.microsoft.com/en-us/dotnet/api/System.IDisposable.Dispose 'System.IDisposable.Dispose')

<a name='TimeProviderExtensions.ManualTimer.Dispose(bool)'></a>

## ManualTimer.Dispose(bool) Method

Disposes of the [ManualTimer](TimeProviderExtensions.ManualTimer.md 'TimeProviderExtensions.ManualTimer') and removes any scheduled callbacks from the [ManualTimeProvider](TimeProviderExtensions.ManualTimeProvider.md 'TimeProviderExtensions.ManualTimeProvider').

```csharp
protected virtual void Dispose(bool disposing);
```
#### Parameters

<a name='TimeProviderExtensions.ManualTimer.Dispose(bool).disposing'></a>

`disposing` [System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')

### Remarks
If this method is overridden, it should always be called by the overriding method.

<a name='TimeProviderExtensions.ManualTimer.DisposeAsync()'></a>

## ManualTimer.DisposeAsync() Method

Disposes of the [ManualTimer](TimeProviderExtensions.ManualTimer.md 'TimeProviderExtensions.ManualTimer') and removes any scheduled callbacks from the [ManualTimeProvider](TimeProviderExtensions.ManualTimeProvider.md 'TimeProviderExtensions.ManualTimeProvider').

```csharp
public System.Threading.Tasks.ValueTask DisposeAsync();
```

Implements [DisposeAsync()](https://docs.microsoft.com/en-us/dotnet/api/System.IAsyncDisposable.DisposeAsync 'System.IAsyncDisposable.DisposeAsync')

#### Returns
[System.Threading.Tasks.ValueTask](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask 'System.Threading.Tasks.ValueTask')

<a name='TimeProviderExtensions.ManualTimer.ToString()'></a>

## ManualTimer.ToString() Method

Returns a string that represents the current object.

```csharp
public override string ToString();
```

#### Returns
[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
A string that represents the current object.