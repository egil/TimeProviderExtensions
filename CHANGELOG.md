# Changelog

All notable changes to TimeScheduler will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.6.0]

- Changed `TestTimeProvider` to `ManualTimeProvider`.
- `ManualTimeProvider` no longer implements on `IDisposable`.
- Moving time forward using `ManualTimeProvider` will now move time forward in steps, stopping at each scheduled timer/callback time, set the internal "UtcNow" clock returned from `GetUtcNow()` invoke the callback, and then progress to the next scheduled timer, until the target "UtcNow" is reached.

## [0.5.0]

- Implemented a shim for the TimeProvider API coming in .NET 8.
- Added support for controlling timestamps during testing.
- Marked the `UtcNow` as obsolete.

## [0.4.0]

- Added support for timers.

## [0.3.0] - 2023-03-03

### Added

- Adds support for cancelling a `CancellationTokenSource` after a specific timespan via the `ITimeScheduler.CancelAfter(CancellationTokenSource, TimeSpan)` method.
- Adds an singleton instance property to `DefaultScheduler` that can be used instead of creating a new instance for every use.

### Changed

- All methods in `DefaultScheduler` marked with the `[MethodImpl(MethodImplOptions.AggressiveInlining)]` attribute.
- `TestScheduler.ForwardTime(TimeSpan time)` throws `ArgumentException` if the `time` argument is not positive.

## [0.2.0] - 2023-02-21

Adds support for `Task.WaitAsync` family of methods.

## [0.1.3-preview] - 2023-01-30

Initial release with support for `Task.Delay` and `PeriodicTimer`.
