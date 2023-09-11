#### [TimeProviderExtensions](index.md 'index')
### [System.Threading](index.md#System.Threading 'System.Threading')

## PeriodicTimerWrapper Class

Provides a lightweight wrapper around a [System.Threading.PeriodicTimer](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.PeriodicTimer 'System.Threading.PeriodicTimer') to enable controlling the timer via a [System.TimeProvider](https://docs.microsoft.com/en-us/dotnet/api/System.TimeProvider 'System.TimeProvider').
A periodic timer enables waiting asynchronously for timer ticks.

```csharp
public abstract class PeriodicTimerWrapper :
System.IDisposable
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; PeriodicTimerWrapper

Implements [System.IDisposable](https://docs.microsoft.com/en-us/dotnet/api/System.IDisposable 'System.IDisposable')

### Remarks

This timer is intended to be used only by a single consumer at a time: only one call to [WaitForNextTickAsync(CancellationToken)](System.Threading.PeriodicTimerWrapper.md#System.Threading.PeriodicTimerWrapper.WaitForNextTickAsync(System.Threading.CancellationToken) 'System.Threading.PeriodicTimerWrapper.WaitForNextTickAsync(System.Threading.CancellationToken)')
may be in flight at any given moment.  [Dispose()](System.Threading.PeriodicTimerWrapper.md#System.Threading.PeriodicTimerWrapper.Dispose() 'System.Threading.PeriodicTimerWrapper.Dispose()') may be used concurrently with an active [WaitForNextTickAsync(CancellationToken)](System.Threading.PeriodicTimerWrapper.md#System.Threading.PeriodicTimerWrapper.WaitForNextTickAsync(System.Threading.CancellationToken) 'System.Threading.PeriodicTimerWrapper.WaitForNextTickAsync(System.Threading.CancellationToken)')
to interrupt it and cause it to return false.
### Methods

<a name='System.Threading.PeriodicTimerWrapper.~PeriodicTimerWrapper()'></a>

## PeriodicTimerWrapper.~PeriodicTimerWrapper() Method

Ensures that resources are freed and other cleanup operations are performed when the garbage collector reclaims the [System.Threading.PeriodicTimer](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.PeriodicTimer 'System.Threading.PeriodicTimer') object.

```csharp
~PeriodicTimerWrapper();
```

<a name='System.Threading.PeriodicTimerWrapper.Dispose()'></a>

## PeriodicTimerWrapper.Dispose() Method

Stops the timer and releases associated managed resources.

```csharp
public void Dispose();
```

Implements [Dispose()](https://docs.microsoft.com/en-us/dotnet/api/System.IDisposable.Dispose 'System.IDisposable.Dispose')

### Remarks
[Dispose()](System.Threading.PeriodicTimerWrapper.md#System.Threading.PeriodicTimerWrapper.Dispose() 'System.Threading.PeriodicTimerWrapper.Dispose()') will cause an active wait with [WaitForNextTickAsync(CancellationToken)](System.Threading.PeriodicTimerWrapper.md#System.Threading.PeriodicTimerWrapper.WaitForNextTickAsync(System.Threading.CancellationToken) 'System.Threading.PeriodicTimerWrapper.WaitForNextTickAsync(System.Threading.CancellationToken)') to complete with a value of false.
            All subsequent [WaitForNextTickAsync(CancellationToken)](System.Threading.PeriodicTimerWrapper.md#System.Threading.PeriodicTimerWrapper.WaitForNextTickAsync(System.Threading.CancellationToken) 'System.Threading.PeriodicTimerWrapper.WaitForNextTickAsync(System.Threading.CancellationToken)') invocations will produce a value of false.

<a name='System.Threading.PeriodicTimerWrapper.Dispose(bool)'></a>

## PeriodicTimerWrapper.Dispose(bool) Method

Dispose of the wrapped [System.Threading.PeriodicTimer](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.PeriodicTimer 'System.Threading.PeriodicTimer').

```csharp
protected abstract void Dispose(bool disposing);
```
#### Parameters

<a name='System.Threading.PeriodicTimerWrapper.Dispose(bool).disposing'></a>

`disposing` [System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')

<a name='System.Threading.PeriodicTimerWrapper.WaitForNextTickAsync(System.Threading.CancellationToken)'></a>

## PeriodicTimerWrapper.WaitForNextTickAsync(CancellationToken) Method

Wait for the next tick of the timer, or for the timer to be stopped.

```csharp
public abstract System.Threading.Tasks.ValueTask<bool> WaitForNextTickAsync(System.Threading.CancellationToken cancellationToken=default(System.Threading.CancellationToken));
```
#### Parameters

<a name='System.Threading.PeriodicTimerWrapper.WaitForNextTickAsync(System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System.Threading.CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.CancellationToken 'System.Threading.CancellationToken')

A [System.Threading.CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.CancellationToken 'System.Threading.CancellationToken') to use to cancel the asynchronous wait. If cancellation is requested, it affects only the single wait operation;
the underlying timer continues firing.

#### Returns
[System.Threading.Tasks.ValueTask&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask-1 'System.Threading.Tasks.ValueTask`1')[System.Boolean](https://docs.microsoft.com/en-us/dotnet/api/System.Boolean 'System.Boolean')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.Tasks.ValueTask-1 'System.Threading.Tasks.ValueTask`1')  
A task that will be completed due to the timer firing, [Dispose()](System.Threading.PeriodicTimerWrapper.md#System.Threading.PeriodicTimerWrapper.Dispose() 'System.Threading.PeriodicTimerWrapper.Dispose()') being called to stop the timer, or cancellation being requested.

### Remarks
The [System.Threading.PeriodicTimer](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.PeriodicTimer 'System.Threading.PeriodicTimer') behaves like an auto-reset event, in that multiple ticks are coalesced into a single tick if they occur between
calls to [WaitForNextTickAsync(CancellationToken)](System.Threading.PeriodicTimerWrapper.md#System.Threading.PeriodicTimerWrapper.WaitForNextTickAsync(System.Threading.CancellationToken) 'System.Threading.PeriodicTimerWrapper.WaitForNextTickAsync(System.Threading.CancellationToken)').  Similarly, a call to [Dispose()](System.Threading.PeriodicTimerWrapper.md#System.Threading.PeriodicTimerWrapper.Dispose() 'System.Threading.PeriodicTimerWrapper.Dispose()') will void any tick not yet consumed. [WaitForNextTickAsync(CancellationToken)](System.Threading.PeriodicTimerWrapper.md#System.Threading.PeriodicTimerWrapper.WaitForNextTickAsync(System.Threading.CancellationToken) 'System.Threading.PeriodicTimerWrapper.WaitForNextTickAsync(System.Threading.CancellationToken)')
may only be used by one consumer at a time, and may be used concurrently with a single call to [Dispose()](System.Threading.PeriodicTimerWrapper.md#System.Threading.PeriodicTimerWrapper.Dispose() 'System.Threading.PeriodicTimerWrapper.Dispose()').