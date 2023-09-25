#### [TimeProviderExtensions](index.md 'index')
### [TimeProviderExtensions](index.md#TimeProviderExtensions 'TimeProviderExtensions')

## AutoAdvanceBehavior Class

The [AutoAdvanceBehavior](TimeProviderExtensions.AutoAdvanceBehavior.md 'TimeProviderExtensions.AutoAdvanceBehavior') type provides a way to enable and customize the automatic advance of time.

```csharp
public sealed class AutoAdvanceBehavior :
System.IEquatable<TimeProviderExtensions.AutoAdvanceBehavior>
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; AutoAdvanceBehavior

Implements [System.IEquatable&lt;](https://docs.microsoft.com/en-us/dotnet/api/System.IEquatable-1 'System.IEquatable`1')[AutoAdvanceBehavior](TimeProviderExtensions.AutoAdvanceBehavior.md 'TimeProviderExtensions.AutoAdvanceBehavior')[&gt;](https://docs.microsoft.com/en-us/dotnet/api/System.IEquatable-1 'System.IEquatable`1')
### Properties

<a name='TimeProviderExtensions.AutoAdvanceBehavior.TimerAutoTriggerCount'></a>

## AutoAdvanceBehavior.TimerAutoTriggerCount Property


Gets or sets the amount of times timer callbacks will automatically be triggered.

Setting this to a number greater than `0` causes any active timers to have their callback invoked until they have been invoked the number of times
specified by [TimerAutoTriggerCount](TimeProviderExtensions.AutoAdvanceBehavior.md#TimeProviderExtensions.AutoAdvanceBehavior.TimerAutoTriggerCount 'TimeProviderExtensions.AutoAdvanceBehavior.TimerAutoTriggerCount'). Before timer callbacks are invoked, time is advanced to match
the time the callback was scheduled to be invoked, just as it is if [Advance(TimeSpan)](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.Advance(System.TimeSpan) 'TimeProviderExtensions.ManualTimeProvider.Advance(System.TimeSpan)')
or [SetUtcNow(DateTimeOffset)](TimeProviderExtensions.ManualTimeProvider.md#TimeProviderExtensions.ManualTimeProvider.SetUtcNow(System.DateTimeOffset) 'TimeProviderExtensions.ManualTimeProvider.SetUtcNow(System.DateTimeOffset)') was manually called.

Setting this to `1` can be used to ensure all timers, e.g. those used by [Task.Delay(TimeSpan, TimeProvider)](https://docs.microsoft.com/en-us/dotnet/api/Task.Delay#Task_Delay_TimeSpan, TimeProvider_ 'Task.Delay(TimeSpan, TimeProvider)'),
[Task.WaitAsync(TimeSpan, TimeProvider)](https://docs.microsoft.com/en-us/dotnet/api/Task.WaitAsync#Task_WaitAsync_TimeSpan, TimeProvider_ 'Task.WaitAsync(TimeSpan, TimeProvider)'), [System.Threading.CancellationTokenSource.CancelAfter(System.TimeSpan)](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.CancellationTokenSource.CancelAfter#System_Threading_CancellationTokenSource_CancelAfter_System_TimeSpan_ 'System.Threading.CancellationTokenSource.CancelAfter(System.TimeSpan)') and others
are completed immediately.

Setting this to a number larger than `1`, e.g. `10`, can be used to automatically cause a [PeriodicTimer(TimeSpan, TimeProvider)](https://docs.microsoft.com/en-us/dotnet/api/PeriodicTimer#PeriodicTimer_TimeSpan, TimeProvider_ 'PeriodicTimer(TimeSpan, TimeProvider)')
to automatically have its [System.Threading.PeriodicTimer.WaitForNextTickAsync(System.Threading.CancellationToken)](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.PeriodicTimer.WaitForNextTickAsync#System_Threading_PeriodicTimer_WaitForNextTickAsync_System_Threading_CancellationToken_ 'System.Threading.PeriodicTimer.WaitForNextTickAsync(System.Threading.CancellationToken)') async enumerable return `10` times.

```csharp
public int TimerAutoTriggerCount { get; set; }
```

#### Property Value
[System.Int32](https://docs.microsoft.com/en-us/dotnet/api/System.Int32 'System.Int32')

#### Exceptions

[System.ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/System.ArgumentOutOfRangeException 'System.ArgumentOutOfRangeException')  
Thrown when set to a value less than zero `0`.

### Remarks
Set to `0` to disable auto timer callback invocation. The default value is zero `0`.

<a name='TimeProviderExtensions.AutoAdvanceBehavior.TimestampAdvanceAmount'></a>

## AutoAdvanceBehavior.TimestampAdvanceAmount Property

Gets or sets the amount of time by which time advances whenever the a timestamp is read via [System.TimeProvider.GetTimestamp](https://docs.microsoft.com/en-us/dotnet/api/System.TimeProvider.GetTimestamp 'System.TimeProvider.GetTimestamp')
or an elapsed time is calculated with [System.TimeProvider.GetElapsedTime(System.Int64)](https://docs.microsoft.com/en-us/dotnet/api/System.TimeProvider.GetElapsedTime#System_TimeProvider_GetElapsedTime_System_Int64_ 'System.TimeProvider.GetElapsedTime(System.Int64)').

```csharp
public System.TimeSpan TimestampAdvanceAmount { get; set; }
```

#### Property Value
[System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan')

#### Exceptions

[System.ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/System.ArgumentOutOfRangeException 'System.ArgumentOutOfRangeException')  
Thrown when set to a value less than [System.TimeSpan.Zero](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan.Zero 'System.TimeSpan.Zero').

### Remarks
Set to [System.TimeSpan.Zero](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan.Zero 'System.TimeSpan.Zero') to disable auto advance. The default value is [System.TimeSpan.Zero](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan.Zero 'System.TimeSpan.Zero').

<a name='TimeProviderExtensions.AutoAdvanceBehavior.UtcNowAdvanceAmount'></a>

## AutoAdvanceBehavior.UtcNowAdvanceAmount Property

Gets or sets the amount of time by which time advances whenever the clock is read via [System.TimeProvider.GetUtcNow](https://docs.microsoft.com/en-us/dotnet/api/System.TimeProvider.GetUtcNow 'System.TimeProvider.GetUtcNow')
or [System.TimeProvider.GetLocalNow](https://docs.microsoft.com/en-us/dotnet/api/System.TimeProvider.GetLocalNow 'System.TimeProvider.GetLocalNow').

```csharp
public System.TimeSpan UtcNowAdvanceAmount { get; set; }
```

#### Property Value
[System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan')

#### Exceptions

[System.ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/System.ArgumentOutOfRangeException 'System.ArgumentOutOfRangeException')  
Thrown when set to a value less than [System.TimeSpan.Zero](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan.Zero 'System.TimeSpan.Zero').

### Remarks
Set to [System.TimeSpan.Zero](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan.Zero 'System.TimeSpan.Zero') to disable auto advance. The default value is [System.TimeSpan.Zero](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan.Zero 'System.TimeSpan.Zero').