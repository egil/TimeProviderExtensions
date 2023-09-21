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

<a name='TimeProviderExtensions.AutoAdvanceBehavior.ClockAdvanceAmount'></a>

## AutoAdvanceBehavior.ClockAdvanceAmount Property

Gets or sets the amount of time by which time advances whenever the clock is read via [System.TimeProvider.GetUtcNow](https://docs.microsoft.com/en-us/dotnet/api/System.TimeProvider.GetUtcNow 'System.TimeProvider.GetUtcNow') or [System.TimeProvider.GetLocalNow](https://docs.microsoft.com/en-us/dotnet/api/System.TimeProvider.GetLocalNow 'System.TimeProvider.GetLocalNow').

```csharp
public System.TimeSpan ClockAdvanceAmount { get; set; }
```

#### Property Value
[System.TimeSpan](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan 'System.TimeSpan')

#### Exceptions

[System.ArgumentOutOfRangeException](https://docs.microsoft.com/en-us/dotnet/api/System.ArgumentOutOfRangeException 'System.ArgumentOutOfRangeException')  
Thrown when set to a value than [System.TimeSpan.Zero](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan.Zero 'System.TimeSpan.Zero').

### Remarks
Set to [System.TimeSpan.Zero](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan.Zero 'System.TimeSpan.Zero') to disable auto advance. The default value is [System.TimeSpan.Zero](https://docs.microsoft.com/en-us/dotnet/api/System.TimeSpan.Zero 'System.TimeSpan.Zero').