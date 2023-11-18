#### [TimeProviderExtensions](index.md 'index')
### [System.Threading](index.md#System.Threading 'System.Threading')

## TimeProviderPeriodicTimerExtensions Class

PeriodicTimer extensions for [System.TimeProvider](https://docs.microsoft.com/en-us/dotnet/api/System.TimeProvider 'System.TimeProvider').

```csharp
public static class TimeProviderPeriodicTimerExtensions
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; TimeProviderPeriodicTimerExtensions
### Methods

<a name='System.Threading.TimeProviderPeriodicTimerExtensions.CreatePeriodicTimer(thisTimeProvider,System.TimeSpan)'></a>

## TimeProviderPeriodicTimerExtensions.CreatePeriodicTimer(this TimeProvider, TimeSpan) Method

Factory method that creates a periodic timer that enables waiting asynchronously for timer ticks.
Use this factory method as a replacement for instantiating a [System.Threading.PeriodicTimer](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.PeriodicTimer 'System.Threading.PeriodicTimer').

```csharp
public static System.Threading.PeriodicTimerWrapper CreatePeriodicTimer(this TimeProvider timeProvider, System.TimeSpan period);
```
#### Parameters

<a name='System.Threading.TimeProviderPeriodicTimerExtensions.CreatePeriodicTimer(thisTimeProvider,System.TimeSpan).timeProvider'></a>

`timeProvider` [System.TimeProvider](https://docs.microsoft.com/en-us/dotnet/api/System.TimeProvider 'System.TimeProvider')

<a name='System.Threading.TimeProviderPeriodicTimerExtensions.CreatePeriodicTimer(thisTimeProvider,System.TimeSpan).period'></a>

`period` [System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan')

#### Returns
[PeriodicTimerWrapper](System.Threading.PeriodicTimerWrapper.md 'System.Threading.PeriodicTimerWrapper')  
A new [PeriodicTimerWrapper](System.Threading.PeriodicTimerWrapper.md 'System.Threading.PeriodicTimerWrapper').
Note, this is a wrapper around a [System.Threading.PeriodicTimer](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.PeriodicTimer 'System.Threading.PeriodicTimer'),
and will behave exactly the same as the original.

### Remarks
This timer is intended to be used only by a single consumer at a time: only one call to [System.Threading.PeriodicTimer.WaitForNextTickAsync(System.Threading.CancellationToken)](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.PeriodicTimer.WaitForNextTickAsync#System_Threading_PeriodicTimer_WaitForNextTickAsync_System_Threading_CancellationToken_ 'System.Threading.PeriodicTimer.WaitForNextTickAsync(System.Threading.CancellationToken)')
may be in flight at any given moment. [System.Threading.PeriodicTimer.Dispose](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.PeriodicTimer.Dispose 'System.Threading.PeriodicTimer.Dispose') may be used concurrently with an active [System.Threading.PeriodicTimer.WaitForNextTickAsync(System.Threading.CancellationToken)](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.PeriodicTimer.WaitForNextTickAsync#System_Threading_PeriodicTimer_WaitForNextTickAsync_System_Threading_CancellationToken_ 'System.Threading.PeriodicTimer.WaitForNextTickAsync(System.Threading.CancellationToken)')
to interrupt it and cause it to return false.