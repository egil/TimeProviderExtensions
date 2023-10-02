#if NET6_0_OR_GREATER

namespace TimeProviderExtensions;

public class ManualTimeProviderWaitAsyncTests
{
    private const uint MaxSupportedTimeout = 0xfffffffe;
    private static readonly TimeSpan DelayedTaskDelay = TimeSpan.FromMilliseconds(2);
    private static readonly string StringTaskResult = Guid.NewGuid().ToString();

    private static async Task DelayedTask(TimeProvider provider) => await provider.Delay(DelayedTaskDelay);

    private static async Task<string> DelayedStringTask(TimeProvider provider)
    {
        await provider.Delay(DelayedTaskDelay);
        return StringTaskResult;
    }

    public static TheoryData<Func<ManualTimeProvider, Task>> TimeoutInvalidInvocations { get; } =
        new TheoryData<Func<ManualTimeProvider, Task>>
        {
            sut => DelayedTask(sut).WaitAsync(TimeSpan.FromMilliseconds(-2), sut),
            sut => DelayedTask(sut).WaitAsync(TimeSpan.FromMilliseconds(-2), sut, CancellationToken.None),
            sut => DelayedStringTask(sut).WaitAsync(TimeSpan.FromMilliseconds(-2), sut),
            sut => DelayedStringTask(sut).WaitAsync(TimeSpan.FromMilliseconds(-2), sut, CancellationToken.None),
            sut => DelayedTask(sut).WaitAsync(TimeSpan.FromMilliseconds(MaxSupportedTimeout + 1), sut),
            sut => DelayedTask(sut).WaitAsync(TimeSpan.FromMilliseconds(MaxSupportedTimeout + 1), sut, CancellationToken.None),
            sut => DelayedStringTask(sut).WaitAsync(TimeSpan.FromMilliseconds(MaxSupportedTimeout + 1), sut),
            sut => DelayedStringTask(sut).WaitAsync(TimeSpan.FromMilliseconds(MaxSupportedTimeout + 1), sut, CancellationToken.None),
        };

    [Theory, MemberData(nameof(TimeoutInvalidInvocations))]
    public async Task WaitAsync_timeout_input_validation(Func<ManualTimeProvider, Task> invalidInvocation)
    {
        var sut = new ManualTimeProvider();

        await sut.Invoking(invalidInvocation)
            .Should()
            .ThrowAsync<ArgumentOutOfRangeException>();
    }

    public static TheoryData<Func<ManualTimeProvider, Task>> CompletedInvocations { get; } =
        new TheoryData<Func<ManualTimeProvider, Task>>
        {
            sut => Task.CompletedTask.WaitAsync(TimeSpan.FromMilliseconds(1), sut),
            sut => Task.CompletedTask.WaitAsync(TimeSpan.FromMilliseconds(1), sut, CancellationToken.None),
            sut => Task<string>.CompletedTask.WaitAsync(TimeSpan.FromMilliseconds(1), sut),
            sut => Task<string>.CompletedTask.WaitAsync(TimeSpan.FromMilliseconds(1), sut, CancellationToken.None),
        };

    [Theory, MemberData(nameof(CompletedInvocations))]
    public void WaitAsync_completes_immediately_when_task_is_completed(Func<ManualTimeProvider, Task> completedInvocation)
    {
        var sut = new ManualTimeProvider();

        var task = completedInvocation(sut);

        task.Should().CompletedSuccessfully();
    }

    public static TheoryData<Func<ManualTimeProvider, Task>> ImmediatelyCanceledInvocations { get; } =
        new TheoryData<Func<ManualTimeProvider, Task>>
        {
            sut => DelayedTask(sut).WaitAsync(TimeSpan.FromMilliseconds(1), sut,new CancellationToken(canceled: true)),
            sut => DelayedStringTask(sut).WaitAsync(TimeSpan.FromMilliseconds(1), sut,new CancellationToken(canceled: true)),
        };

    [Theory, MemberData(nameof(ImmediatelyCanceledInvocations))]
    public void WaitAsync_canceled_immediately_when_cancellationToken_is_set(Func<ManualTimeProvider, Task> canceledInvocation)
    {
        var sut = new ManualTimeProvider();

        var task = canceledInvocation(sut);

        task.Should().Canceled();
    }

    public static TheoryData<Func<ManualTimeProvider, Task>> ImmediateTimedoutInvocations { get; } =
        new TheoryData<Func<ManualTimeProvider, Task>>
        {
            sut => DelayedTask(sut).WaitAsync(TimeSpan.Zero, sut),
            sut => DelayedTask(sut).WaitAsync(TimeSpan.Zero, sut, CancellationToken.None),
            sut => DelayedStringTask(sut).WaitAsync(TimeSpan.Zero, sut),
            sut => DelayedStringTask(sut).WaitAsync(TimeSpan.Zero, sut, CancellationToken.None),
        };

    [Theory, MemberData(nameof(ImmediateTimedoutInvocations))]
    public async Task WaitAsync_throws_immediately_when_timeout_is_zero(Func<ManualTimeProvider, Task> immediateTimedoutInvocation)
    {
        var sut = new ManualTimeProvider();

        await sut.Invoking(immediateTimedoutInvocation)
            .Should()
            .ThrowExactlyAsync<TimeoutException>();
    }

    public static TheoryData<Func<ManualTimeProvider, Task>> ValidInvocations { get; } =
        new TheoryData<Func<ManualTimeProvider, Task>>
        {
            sut => DelayedTask(sut).WaitAsync(TimeSpan.FromSeconds(1), sut),
            sut => DelayedTask(sut).WaitAsync(TimeSpan.FromSeconds(1), sut, CancellationToken.None),
            sut => DelayedStringTask(sut).WaitAsync(TimeSpan.FromSeconds(1), sut),
            sut => DelayedStringTask(sut).WaitAsync(TimeSpan.FromSeconds(1), sut, CancellationToken.None),
        };

    [Theory, MemberData(nameof(ValidInvocations))]
    public async Task WaitAsync_completes_successfully(Func<ManualTimeProvider, Task> validInvocation)
    {
        var sut = new ManualTimeProvider();
        var task = validInvocation(sut);

        sut.Advance(DelayedTaskDelay);

        await task
            .Should()
            .CompletedSuccessfullyAsync();
    }

    public static TheoryData<Func<ManualTimeProvider, Task<string>>> ValidStringInvocations { get; } =
        new TheoryData<Func<ManualTimeProvider, Task<string>>>
        {
            sut => DelayedStringTask(sut).WaitAsync(TimeSpan.FromSeconds(1), sut),
            sut => DelayedStringTask(sut).WaitAsync(TimeSpan.FromSeconds(1), sut, CancellationToken.None),
        };

    [Theory, MemberData(nameof(ValidStringInvocations))]
    public async Task WaitAsync_of_T_completes_successfully(Func<ManualTimeProvider, Task<string>> validInvocation)
    {
        var sut = new ManualTimeProvider();
        var task = validInvocation(sut);

        sut.Advance(DelayedTaskDelay);

        await task
            .Should()
            .CompletedSuccessfullyAsync();
        task.Result.Should().Be(StringTaskResult);
    }

    public static TheoryData<Func<ManualTimeProvider, TimeSpan, Task>> TimedoutInvocations { get; } =
        new TheoryData<Func<ManualTimeProvider, TimeSpan, Task>>
        {
            (sut, timeout) => DelayedTask(sut).WaitAsync(timeout, sut),
            (sut, timeout) => DelayedTask(sut).WaitAsync(timeout, sut, CancellationToken.None),
            (sut, timeout) => DelayedStringTask(sut).WaitAsync(timeout, sut),
            (sut, timeout) => DelayedStringTask(sut).WaitAsync(timeout, sut, CancellationToken.None),
        };

    [Theory, MemberData(nameof(TimedoutInvocations))]
    public async Task WaitAsync_throws_when_timeout_is_reached(Func<ManualTimeProvider, TimeSpan, Task> invocationWithTime)
    {
        var sut = new ManualTimeProvider();
        var task = invocationWithTime(sut, TimeSpan.FromMilliseconds(1));

        sut.Advance(TimeSpan.FromMilliseconds(1));

        await task.Awaiting(x => x)
            .Should()
            .ThrowExactlyAsync<TimeoutException>();
    }

    public static TheoryData<Func<ManualTimeProvider, CancellationToken, Task>> CancelledInvocations { get; } =
        new TheoryData<Func<ManualTimeProvider, CancellationToken, Task>>
        {
            (sut, token) => DelayedTask(sut).WaitAsync(TimeSpan.FromMilliseconds(1), sut, token),
            (sut, token) => DelayedStringTask(sut).WaitAsync(TimeSpan.FromMilliseconds(1), sut, token),
        };

    [Theory, MemberData(nameof(CancelledInvocations))]
    public async Task WaitAsync_throws_when_token_is_canceled(Func<ManualTimeProvider, CancellationToken, Task> invocationWithCancelToken)
    {
        using var cts = new CancellationTokenSource();
        var sut = new ManualTimeProvider();
        var task = invocationWithCancelToken(sut, cts.Token);

        cts.Cancel();

        await task.Awaiting(x => x)
            .Should()
            .ThrowExactlyAsync<TaskCanceledException>();
    }

    [Fact]
    public void WaitAsync_with_TimerAutoInvokeCount_gt_zero()
    {
        var tcs = new TaskCompletionSource();
        var sut = new ManualTimeProvider { AutoAdvanceBehavior = { TimerAutoTriggerCount= 1 } };

        var task = tcs.Task.WaitAsync(1.Seconds(), sut);

        task.Status.Should().Be(TaskStatus.Faulted);
        task.Exception!.InnerException.Should().BeOfType<TimeoutException>();
        sut.ActiveTimers.Should().Be(0);
    }

    [Fact]
    public async Task Active_timer_with_TimerAutoAdvanceTimes_one_other_thread()
    {
        var sut = new ManualTimeProvider { AutoAdvanceBehavior = { TimerAutoTriggerCount = 1 } };
        var tcs = new TaskCompletionSource();
        using var t1 = sut.CreateTimer(_ => { }, null, 1.Seconds(), 1.Seconds());
        using var t2 = sut.CreateTimer(_ => { }, null, 1.Minutes(), 1.Minutes());

        var task = Task.Run(async () => await tcs.Task.WaitAsync(10.Seconds(), sut));

        try
        {
            await task;
        }
        catch
        {
            task.Status.Should().Be(TaskStatus.Faulted);
            task.Exception!.InnerException.Should().BeOfType<TimeoutException>();
        }

        sut.ActiveTimers.Should().Be(2);
    }
}
#endif