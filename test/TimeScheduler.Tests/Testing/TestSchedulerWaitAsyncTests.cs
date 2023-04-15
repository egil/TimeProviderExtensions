namespace TimeScheduler.Testing;

[Obsolete] // marked obsolete to stop warnings related to SUT
public class TestSchedulerWaitAsyncTests
{
    internal const uint MaxSupportedTimeout = 0xfffffffe;
    private readonly static TimeSpan DelayedTaskDelay = TimeSpan.FromMilliseconds(2);
    private readonly static string StringTaskResult = Guid.NewGuid().ToString();

    private static async Task DelayedTask(ITimeScheduler scheduler)
    {
        await scheduler.Delay(DelayedTaskDelay);
    }

    private static async Task<string> DelayedStringTask(ITimeScheduler scheduler)
    {
        await scheduler.Delay(DelayedTaskDelay);
        return StringTaskResult;
    }

    public static TheoryData<Func<TestScheduler, Task>> NullTaskInvalidInvocations { get; } =
        new TheoryData<Func<TestScheduler, Task>>
        {
            sut => sut.WaitAsync(default(Task)!, TimeSpan.FromSeconds(1)),
            sut => sut.WaitAsync(default(Task)!, TimeSpan.FromSeconds(1), CancellationToken.None),
            sut => sut.WaitAsync(default(Task<string>)!, TimeSpan.FromSeconds(1)),
            sut => sut.WaitAsync(default(Task<string>)!, TimeSpan.FromSeconds(1), CancellationToken.None),
        };

    [Theory, MemberData(nameof(NullTaskInvalidInvocations))]
    public async Task WaitAsync_task_input_validation(Func<TestScheduler, Task> invalidInvocation)
    {
        using var sut = new TestScheduler();

        await sut.Invoking(invalidInvocation)
            .Should()
            .ThrowAsync<ArgumentNullException>()
            .WithParameterName("task");
    }

    public static TheoryData<Func<TestScheduler, Task>> TimeoutInvalidInvocations { get; } =
        new TheoryData<Func<TestScheduler, Task>>
        {
            sut => sut.WaitAsync(DelayedTask(sut), TimeSpan.FromMilliseconds(-2)),
            sut => sut.WaitAsync(DelayedTask(sut), TimeSpan.FromMilliseconds(-2), CancellationToken.None),
            sut => sut.WaitAsync(DelayedStringTask(sut), TimeSpan.FromMilliseconds(-2)),
            sut => sut.WaitAsync(DelayedStringTask(sut), TimeSpan.FromMilliseconds(-2), CancellationToken.None),
            sut => sut.WaitAsync(DelayedTask(sut), TimeSpan.FromMilliseconds(MaxSupportedTimeout + 1)),
            sut => sut.WaitAsync(DelayedTask(sut), TimeSpan.FromMilliseconds(MaxSupportedTimeout + 1), CancellationToken.None),
            sut => sut.WaitAsync(DelayedStringTask(sut), TimeSpan.FromMilliseconds(MaxSupportedTimeout + 1)),
            sut => sut.WaitAsync(DelayedStringTask(sut), TimeSpan.FromMilliseconds(MaxSupportedTimeout + 1), CancellationToken.None),
        };

    [Theory, MemberData(nameof(TimeoutInvalidInvocations))]
    public async Task WaitAsync_timeout_input_validation(Func<TestScheduler, Task> invalidInvocation)
    {
        using var sut = new TestScheduler();

        await sut.Invoking(invalidInvocation)
            .Should()
            .ThrowAsync<ArgumentOutOfRangeException>()
            .WithParameterName("timeout");
    }

    public static TheoryData<Func<TestScheduler, Task>> CompletedInvocations { get; } =
        new TheoryData<Func<TestScheduler, Task>>
        {
            sut => sut.WaitAsync(Task.CompletedTask, TimeSpan.FromMilliseconds(1)),
            sut => sut.WaitAsync(Task.CompletedTask, TimeSpan.FromMilliseconds(1), CancellationToken.None),
            sut => sut.WaitAsync(Task<string>.CompletedTask, TimeSpan.FromMilliseconds(1)),
            sut => sut.WaitAsync(Task<string>.CompletedTask, TimeSpan.FromMilliseconds(1), CancellationToken.None),
        };

    [Theory, MemberData(nameof(CompletedInvocations))]
    public void WaitAsync_completes_immediately_when_task_is_completed(Func<TestScheduler, Task> completedInvocation)
    {
        using var sut = new TestScheduler();

        var task = completedInvocation(sut);

        task.Should().CompletedSuccessfully();
    }

    public static TheoryData<Func<TestScheduler, Task>> ImmediatelyCanceledInvocations { get; } =
        new TheoryData<Func<TestScheduler, Task>>
        {
            sut => sut.WaitAsync(DelayedTask(sut), TimeSpan.FromMilliseconds(1), new CancellationToken(canceled: true)),
            sut => sut.WaitAsync(DelayedStringTask(sut), TimeSpan.FromMilliseconds(1), new CancellationToken(canceled: true)),
        };

    [Theory, MemberData(nameof(ImmediatelyCanceledInvocations))]
    public void WaitAsync_canceled_immediately_when_cancellationToken_is_set(Func<TestScheduler, Task> canceledInvocation)
    {
        using var sut = new TestScheduler();

        var task = canceledInvocation(sut);

        task.Should().Canceled();
    }

    public static TheoryData<Func<TestScheduler, Task>> ImmediateTimedoutInvocations { get; } =
        new TheoryData<Func<TestScheduler, Task>>
        {
            sut => sut.WaitAsync(DelayedTask(sut), TimeSpan.Zero),
            sut => sut.WaitAsync(DelayedTask(sut), TimeSpan.Zero, CancellationToken.None),
            sut => sut.WaitAsync(DelayedStringTask(sut), TimeSpan.Zero),
            sut => sut.WaitAsync(DelayedStringTask(sut), TimeSpan.Zero, CancellationToken.None),
        };

    [Theory, MemberData(nameof(ImmediateTimedoutInvocations))]
    public async Task WaitAsync_throws_immediately_when_timeout_is_zero(Func<TestScheduler, Task> immediateTimedoutInvocation)
    {
        using var sut = new TestScheduler();

        await sut.Invoking(immediateTimedoutInvocation)
            .Should()
            .ThrowExactlyAsync<TimeoutException>();
    }


    public static TheoryData<Func<TestScheduler, Task>> ValidInvocations { get; } =
        new TheoryData<Func<TestScheduler, Task>>
        {
            sut => sut.WaitAsync(DelayedTask(sut), TimeSpan.FromSeconds(1)),
            sut => sut.WaitAsync(DelayedTask(sut), TimeSpan.FromSeconds(1), CancellationToken.None),
            sut => sut.WaitAsync(DelayedStringTask(sut), TimeSpan.FromSeconds(1)),
            sut => sut.WaitAsync(DelayedStringTask(sut), TimeSpan.FromSeconds(1), CancellationToken.None),
        };

    [Theory, MemberData(nameof(ValidInvocations))]
    public async Task WaitAsync_completes_successfully(Func<TestScheduler, Task> validInvocation)
    {
        using var sut = new TestScheduler();
        var task = validInvocation(sut);

        sut.ForwardTime(DelayedTaskDelay);

        await task
            .Should()
            .CompletedSuccessfullyAsync();
    }

    public static TheoryData<Func<TestScheduler, Task<string>>> ValidStringInvocations { get; } =
        new TheoryData<Func<TestScheduler, Task<string>>>
        {
            sut => sut.WaitAsync(DelayedStringTask(sut), TimeSpan.FromSeconds(1)),
            sut => sut.WaitAsync(DelayedStringTask(sut), TimeSpan.FromSeconds(1), CancellationToken.None),
        };

    [Theory, MemberData(nameof(ValidStringInvocations))]
    public async Task WaitAsync_of_T_completes_successfully(Func<TestScheduler, Task<string>> validInvocation)
    {
        using var sut = new TestScheduler();
        var task = validInvocation(sut);

        sut.ForwardTime(DelayedTaskDelay);

        await task
            .Should()
            .CompletedSuccessfullyAsync();
        task.Result.Should().Be(StringTaskResult);
    }

    public static TheoryData<Func<TestScheduler, TimeSpan, Task>> TimedoutInvocations { get; } =
        new TheoryData<Func<TestScheduler, TimeSpan, Task>>
        {
            (sut, timeout) => sut.WaitAsync(DelayedTask(sut), timeout),
            (sut, timeout) => sut.WaitAsync(DelayedTask(sut), timeout, CancellationToken.None),
            (sut, timeout) => sut.WaitAsync(DelayedStringTask(sut), timeout),
            (sut, timeout) => sut.WaitAsync(DelayedStringTask(sut), timeout, CancellationToken.None),
        };

    [Theory, MemberData(nameof(TimedoutInvocations))]
    public async Task WaitAsync_throws_when_timeout_is_reached(Func<TestScheduler, TimeSpan, Task> invocationWithTime)
    {
        using var sut = new TestScheduler();
        var task = invocationWithTime(sut, TimeSpan.FromMilliseconds(1));

        sut.ForwardTime(TimeSpan.FromMilliseconds(1));

        await task.Awaiting(x => x)
            .Should()
            .ThrowExactlyAsync<TimeoutException>();
    }

    public static TheoryData<Func<TestScheduler, CancellationToken, Task>> CancelledInvocations { get; } =
        new TheoryData<Func<TestScheduler, CancellationToken, Task>>
        {
            (sut, token) => sut.WaitAsync(DelayedTask(sut), TimeSpan.FromMilliseconds(1), token),
            (sut, token) => sut.WaitAsync(DelayedStringTask(sut), TimeSpan.FromMilliseconds(1), token),
        };

    [Theory, MemberData(nameof(CancelledInvocations))]
    public async Task WaitAsync_throws_when_token_is_canceled(Func<TestScheduler, CancellationToken, Task> invocationWithCancelToken)
    {
        using var cts = new CancellationTokenSource();
        using var sut = new TestScheduler();
        var task = invocationWithCancelToken(sut, cts.Token);

        cts.Cancel();

        await task.Awaiting(x => x)
            .Should()
            .ThrowExactlyAsync<TaskCanceledException>();
    }
}
