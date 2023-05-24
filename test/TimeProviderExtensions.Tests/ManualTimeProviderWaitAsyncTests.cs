#if NET6_0_OR_GREATER
#if TargetMicrosoftTestTimeProvider
using SutTimeProvider = Microsoft.Extensions.Time.Testing.FakeTimeProvider;
#else
using SutTimeProvider = TimeProviderExtensions.ManualTimeProvider;
#endif

namespace TimeProviderExtensions;

internal class ManualTimeProviderWaitAsyncTests
{
    internal const uint MaxSupportedTimeout = 0xfffffffe;
    private readonly static TimeSpan DelayedTaskDelay = TimeSpan.FromMilliseconds(2);
    private readonly static string StringTaskResult = Guid.NewGuid().ToString();

    private static async Task DelayedTask(TimeProvider provider)
    {
        await provider.Delay(DelayedTaskDelay);
    }

    private static async Task<string> DelayedStringTask(TimeProvider provider)
    {
        await provider.Delay(DelayedTaskDelay);
        return StringTaskResult;
    }

    public static TheoryData<Func<SutTimeProvider, Task>> NullTaskInvalidInvocations { get; } =
        new TheoryData<Func<SutTimeProvider, Task>>
        {
            sut => default(Task)!.WaitAsync(TimeSpan.FromSeconds(1), sut),
            sut => default(Task)!.WaitAsync(TimeSpan.FromSeconds(1), sut, CancellationToken.None),
            sut => default(Task<string>)!.WaitAsync(TimeSpan.FromSeconds(1), sut),
            sut => default(Task<string>)!.WaitAsync(TimeSpan.FromSeconds(1), sut, CancellationToken.None),
        };

    [Theory(Skip = "Not implemented in Microsofts implementation"), MemberData(nameof(NullTaskInvalidInvocations))]
    public async Task WaitAsync_task_input_validation(Func<SutTimeProvider, Task> invalidInvocation)
    {
        var sut = new SutTimeProvider();

        await sut.Invoking(invalidInvocation)
            .Should()
            .ThrowAsync<ArgumentNullException>()
            .WithParameterName("task");
    }

    public static TheoryData<Func<SutTimeProvider, Task>> TimeoutInvalidInvocations { get; } =
        new TheoryData<Func<SutTimeProvider, Task>>
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
    public async Task WaitAsync_timeout_input_validation(Func<SutTimeProvider, Task> invalidInvocation)
    {
        var sut = new SutTimeProvider();

        await sut.Invoking(invalidInvocation)
            .Should()
            .ThrowAsync<ArgumentOutOfRangeException>();
        // This should be timeout in all cases, but it seems the .NET 8 implementation sometimes returns dueTime.
        //.WithParameterName("timeout"); 
    }

    public static TheoryData<Func<SutTimeProvider, Task>> CompletedInvocations { get; } =
        new TheoryData<Func<SutTimeProvider, Task>>
        {
            sut => Task.CompletedTask.WaitAsync(TimeSpan.FromMilliseconds(1), sut),
            sut => Task.CompletedTask.WaitAsync(TimeSpan.FromMilliseconds(1), sut, CancellationToken.None),
            sut => Task<string>.CompletedTask.WaitAsync(TimeSpan.FromMilliseconds(1), sut),
            sut => Task<string>.CompletedTask.WaitAsync(TimeSpan.FromMilliseconds(1), sut, CancellationToken.None),
        };

    [Theory, MemberData(nameof(CompletedInvocations))]
    public void WaitAsync_completes_immediately_when_task_is_completed(Func<SutTimeProvider, Task> completedInvocation)
    {
        var sut = new SutTimeProvider();

        var task = completedInvocation(sut);

        task.Should().CompletedSuccessfully();
    }

    public static TheoryData<Func<SutTimeProvider, Task>> ImmediatelyCanceledInvocations { get; } =
        new TheoryData<Func<SutTimeProvider, Task>>
        {
            sut => DelayedTask(sut).WaitAsync(TimeSpan.FromMilliseconds(1), sut,new CancellationToken(canceled: true)),
            sut => DelayedStringTask(sut).WaitAsync(TimeSpan.FromMilliseconds(1), sut,new CancellationToken(canceled: true)),
        };

    [Theory, MemberData(nameof(ImmediatelyCanceledInvocations))]
    public void WaitAsync_canceled_immediately_when_cancellationToken_is_set(Func<SutTimeProvider, Task> canceledInvocation)
    {
        var sut = new SutTimeProvider();

        var task = canceledInvocation(sut);

        task.Should().Canceled();
    }

    public static TheoryData<Func<SutTimeProvider, Task>> ImmediateTimedoutInvocations { get; } =
        new TheoryData<Func<SutTimeProvider, Task>>
        {
            sut => DelayedTask(sut).WaitAsync(TimeSpan.Zero, sut),
            sut => DelayedTask(sut).WaitAsync(TimeSpan.Zero, sut, CancellationToken.None),
            sut => DelayedStringTask(sut).WaitAsync(TimeSpan.Zero, sut),
            sut => DelayedStringTask(sut).WaitAsync(TimeSpan.Zero, sut, CancellationToken.None),
        };

    [Theory(Skip = "Not implemented in Microsofts implementation"), MemberData(nameof(ImmediateTimedoutInvocations))]
    public async Task WaitAsync_throws_immediately_when_timeout_is_zero(Func<SutTimeProvider, Task> immediateTimedoutInvocation)
    {
        var sut = new SutTimeProvider();

        await sut.Invoking(immediateTimedoutInvocation)
            .Should()
            .ThrowExactlyAsync<TimeoutException>();
    }

    public static TheoryData<Func<SutTimeProvider, Task>> ValidInvocations { get; } =
        new TheoryData<Func<SutTimeProvider, Task>>
        {
            sut => DelayedTask(sut).WaitAsync(TimeSpan.FromSeconds(1), sut),
            sut => DelayedTask(sut).WaitAsync(TimeSpan.FromSeconds(1), sut, CancellationToken.None),
            sut => DelayedStringTask(sut).WaitAsync(TimeSpan.FromSeconds(1), sut),
            sut => DelayedStringTask(sut).WaitAsync(TimeSpan.FromSeconds(1), sut, CancellationToken.None),
        };

    [Theory, MemberData(nameof(ValidInvocations))]
    public async Task WaitAsync_completes_successfully(Func<SutTimeProvider, Task> validInvocation)
    {
        var sut = new SutTimeProvider();
        var task = validInvocation(sut);

        sut.Advance(DelayedTaskDelay);

        await task
            .Should()
            .CompletedSuccessfullyAsync();
    }

    public static TheoryData<Func<SutTimeProvider, Task<string>>> ValidStringInvocations { get; } =
        new TheoryData<Func<SutTimeProvider, Task<string>>>
        {
            sut => DelayedStringTask(sut).WaitAsync(TimeSpan.FromSeconds(1), sut),
            sut => DelayedStringTask(sut).WaitAsync(TimeSpan.FromSeconds(1), sut, CancellationToken.None),
        };

    [Theory, MemberData(nameof(ValidStringInvocations))]
    public async Task WaitAsync_of_T_completes_successfully(Func<SutTimeProvider, Task<string>> validInvocation)
    {
        var sut = new SutTimeProvider();
        var task = validInvocation(sut);

        sut.Advance(DelayedTaskDelay);

        await task
            .Should()
            .CompletedSuccessfullyAsync();
        task.Result.Should().Be(StringTaskResult);
    }

    public static TheoryData<Func<SutTimeProvider, TimeSpan, Task>> TimedoutInvocations { get; } =
        new TheoryData<Func<SutTimeProvider, TimeSpan, Task>>
        {
            (sut, timeout) => DelayedTask(sut).WaitAsync(timeout, sut),
            (sut, timeout) => DelayedTask(sut).WaitAsync(timeout, sut, CancellationToken.None),
            (sut, timeout) => DelayedStringTask(sut).WaitAsync(timeout, sut),
            (sut, timeout) => DelayedStringTask(sut).WaitAsync(timeout, sut, CancellationToken.None),
        };

    [Theory, MemberData(nameof(TimedoutInvocations))]
    public async Task WaitAsync_throws_when_timeout_is_reached(Func<SutTimeProvider, TimeSpan, Task> invocationWithTime)
    {
        var sut = new SutTimeProvider();
        var task = invocationWithTime(sut, TimeSpan.FromMilliseconds(1));

        sut.Advance(TimeSpan.FromMilliseconds(1));

        await task.Awaiting(x => x)
            .Should()
            .ThrowExactlyAsync<TimeoutException>();
    }

    public static TheoryData<Func<SutTimeProvider, CancellationToken, Task>> CancelledInvocations { get; } =
        new TheoryData<Func<SutTimeProvider, CancellationToken, Task>>
        {
            (sut, token) => DelayedTask(sut).WaitAsync(TimeSpan.FromMilliseconds(1), sut, token),
            (sut, token) => DelayedStringTask(sut).WaitAsync(TimeSpan.FromMilliseconds(1), sut, token),
        };

    [Theory, MemberData(nameof(CancelledInvocations))]
    public async Task WaitAsync_throws_when_token_is_canceled(Func<SutTimeProvider, CancellationToken, Task> invocationWithCancelToken)
    {
        using var cts = new CancellationTokenSource();
        var sut = new SutTimeProvider();
        var task = invocationWithCancelToken(sut, cts.Token);

        cts.Cancel();

        await task.Awaiting(x => x)
            .Should()
            .ThrowExactlyAsync<TaskCanceledException>();
    }
}
#endif