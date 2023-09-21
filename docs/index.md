#### [TimeProviderExtensions](index.md 'index')

## TimeProviderExtensions Assembly
### Namespaces

<a name='System.Threading'></a>

## System.Threading Namespace

| Classes | |
| :--- | :--- |
| [PeriodicTimerWrapper](System.Threading.PeriodicTimerWrapper.md 'System.Threading.PeriodicTimerWrapper') | Provides a lightweight wrapper around a [System.Threading.PeriodicTimer](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.PeriodicTimer 'System.Threading.PeriodicTimer') to enable controlling the timer via a [System.TimeProvider](https://docs.microsoft.com/en-us/dotnet/api/System.TimeProvider 'System.TimeProvider'). A periodic timer enables waiting asynchronously for timer ticks. |
| [TimeProviderPeriodicTimerExtensions](System.Threading.TimeProviderPeriodicTimerExtensions.md 'System.Threading.TimeProviderPeriodicTimerExtensions') | PeriodicTimer extensions for [System.TimeProvider](https://docs.microsoft.com/en-us/dotnet/api/System.TimeProvider 'System.TimeProvider'). |

<a name='TimeProviderExtensions'></a>

## TimeProviderExtensions Namespace

| Classes | |
| :--- | :--- |
| [AutoAdvanceBehavior](TimeProviderExtensions.AutoAdvanceBehavior.md 'TimeProviderExtensions.AutoAdvanceBehavior') | The [AutoAdvanceBehavior](TimeProviderExtensions.AutoAdvanceBehavior.md 'TimeProviderExtensions.AutoAdvanceBehavior') type provides a way to enable and customize the automatic advance of time. |
| [ManualTimeProvider](TimeProviderExtensions.ManualTimeProvider.md 'TimeProviderExtensions.ManualTimeProvider') | Represents a synthetic time provider that can be used to enable deterministic behavior in tests. |
| [ManualTimer](TimeProviderExtensions.ManualTimer.md 'TimeProviderExtensions.ManualTimer') | A implementation of a [System.Threading.ITimer](https://docs.microsoft.com/en-us/dotnet/api/System.Threading.ITimer 'System.Threading.ITimer') whose callbacks are scheduled via a [ManualTimeProvider](TimeProviderExtensions.ManualTimeProvider.md 'TimeProviderExtensions.ManualTimeProvider'). |
