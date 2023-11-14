# Changelog

All notable changes to TimeProviderExtensions will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0]

- Upgrade dependencies to none-preview versions.

## [1.0.0-rc.3]

- Generate strong-named assemblies.

## [1.0.0-rc.2]

- Added `ActiveTimers` property to `ManualTimeProvider`. The property will display the number of currently active timers that have a callback scheduled to be called in the future.

- Allow `ManualTimeProvider.Start` to be set using property initializers.

- Made the timer type created by `ManualTimeProvider`, the `ManualTimer` type, public, and introduced a protected method `CreateManualTimer` on `ManualTimeProvider`. This enables advanced scenarios where a custom `ManualTimer` is needed.

  A custom implementation of `ManualTimer` can override the `Change` method and add custom behavior to it.

  Overriding `CreateManualTimer` makes it possible to intercept a `TimerCallback` and perform actions before and after the timer callback has been invoked.

- Replace the `AutoAdvanceAmount` property with the `AutoAdvanceBehavior` property on `ManualTimeProvider`, and introduce the `AutoAdvanceBehavior` type. To automatically advance the time when `GetUtcNow()` or `GetLocalNow()` is called, set `AutoAdvanceBehavior.UtcNowAdvanceAmount` to a `TimeSpan` larger than zero.

- Enable auto advance feature for `GetTimestamp()` and `GetElapsedTime(long)`. To automatically advance the time when `GetTimestamp()` or `GetElapsedTime(long)` is called, set `AutoAdvanceBehavior.TimestampAdvanceAmount` to a `TimeSpan` larger than zero.

- `ManualTimer` now exposes its current configuration. `DueTime`, `Period`, `IsActive`, `CallbackTime`, and `CallbackInvokeCount` are now publicly visible.

- Enable auto-advance feature for timers. This enables automatically calling timers callback a specified number of times, by setting the `AutoAdvanceBehavior.TimerAutoTriggerCount` property to a number larger than zero.

## [1.0.0-rc.1]

- Updated Microsoft.Bcl.TimeProvider package dependency to rc.1 version.

## [1.0.0-preview.7]

- Added support for netstandard2.0, as this is supported by the back-port package https://www.nuget.org/packages/Microsoft.Bcl.TimeProvider.

## [1.0.0-preview.6]

- Added `Jump(TimeSpan)` and `Jump(DateTimeOffset)` methods that will jump time to the specified place. Any timer callbacks between the start and end of the jump will be invoked the expected number of times, but the date/time returned from `GetUtcNow()` and `GetTimestamp()` will always be the jump time. This differs from how `Advance` and `SetUtcNow` works. See the readme for a detailed description.

## [1.0.0-preview.5]

Aligned the public API surface of `ManualTimeProvider` with `Microsoft.Extensions.Time.Testing.FakeTimeProvider`. This means:

  - The `StartTime` property is now called `Start`.
  - The `ForwardTime` method has been removed (use `Advance` instead).
  - The `AutoAdvanceAmount` property has been introduced, which will advance time with the specified amount every time `GetUtcNow()` is called. It defaults to `TimeSpan.Zero`, which disables auto-advancing.

## [1.0.0-preview.4]

- Added 'StartTime' to `ManualTestProvider`, which represents the initial date/time when the `ManualtTimeProvider` was initialized.

## [1.0.0-preview.3]

- Changed `ManualTestProvider` to set the local time zone to UTC by default, providing a method for overriding during testing.

- Changed the `ManualTestProvider.ToString()` method to return current date time.

- Fixed `ITimer` returned by `ManualTestProvider` such that timers created with a due time equal to zero will fire the timer callback immediately.

## [1.0.0-preview.1]

This release adds a dependency on [Microsoft.Bcl.TimeProvider](https://www.nuget.org/packages/Microsoft.Bcl.TimeProvider) and utilizes the types built-in to that to do much of the work.

When using the `ManualTimeProvider` during testing, be aware of these outstanding issues: https://github.com/dotnet/runtime/issues/85326

- Removed `CancelAfter` extension methods. Instead, create a CancellationTokenSource via the method `TimeProvider.CreateCancellationTokenSource(TimeSpan delay)` or in .NET 8, using `new CancellationTokenSource(TimeSpan delay, TimeProvider timeProvider).

  **NOTE:** If running on .NET versions earlier than .NET 8.0, there is a constraint when invoking `CancellationTokenSource.CancelAfter(TimeSpan)` on the resultant object. This action will not terminate the initial timer indicated by `delay`. However, this restriction does not apply to .NET 8.0 and later versions.

## [0.8.0]

- Added `TimeProvider.GetElapsedTime(long startingTimestamp)`
- Added `TimeProvider.CreateCancellationTokenSource(TimeSpan delay)`

## [0.7.0]

- Add support for libraries that target netstandard 2.0.

## [0.6.0]

- Changed `TestTimeProvider` to `ManualTimeProvider`.
- `ManualTimeProvider` no longer implements on `IDisposable`.
- Moving time forward using `ManualTimeProvider` will now move time forward in steps, stopping at each scheduled timer/callback time, setting the internal "UtcNow" clock returned from `GetUtcNow()` to invoke the callback, and then progress to the next scheduled timer, until the target "UtcNow" is reached.

## [0.5.0]

- Implemented a shim for the TimeProvider API coming in .NET 8.
- Added support for controlling timestamps during testing.
- Marked the `UtcNow` as obsolete.

## [0.4.0]

- Added support for timers.

## [0.3.0] - 2023-03-03

### Added

- Adds support for canceling a `CancellationTokenSource` after a specific timespan via the `ITimeScheduler.CancelAfter(CancellationTokenSource, TimeSpan)` method.
- Adds a singleton instance property to `DefaultScheduler` that can be used instead of creating a new instance for every use.

### Changed

- All methods in `DefaultScheduler` marked with the `[MethodImpl(MethodImplOptions.AggressiveInlining)]` attribute.
- `TestScheduler.ForwardTime(TimeSpan time)` throws `ArgumentException` if the `time` argument is not positive.

## [0.2.0] - 2023-02-21

Adds support for the `Task.WaitAsync` family of methods.

## [0.1.3-preview] - 2023-01-30

Initial release with support for `Task.Delay` and `PeriodicTimer`.
