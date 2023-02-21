using FluentAssertions.Primitives;

namespace FluentAssertions;

internal static class TaskAssertionsExtensions
{
    public static TaskAssertions Should(this Task subject)
    {
        return new TaskAssertions(subject);
    }
}

public class TaskAssertions : ReferenceTypeAssertions<Task, TaskAssertions>
{
    public TaskAssertions(Task subject) : base(subject)
    {
    }

    protected override string Identifier => "task";

    public AndConstraint<TaskAssertions> CompletedSuccessfully(string because = "", params object[] becauseArgs)
    {
        Subject.IsCompletedSuccessfully.Should().BeTrue(because, becauseArgs);
        return new AndConstraint<TaskAssertions>(this);
    }

    public AndConstraint<TaskAssertions> Canceled(string because = "", params object[] becauseArgs)
    {
        Subject.IsCanceled.Should().BeTrue(because, becauseArgs);
        return new AndConstraint<TaskAssertions>(this);
    }


    public async Task<AndConstraint<TaskAssertions>> CompletedSuccessfullyAsync(string because = "", params object[] becauseArgs)
    {
        await Subject;
        Subject.IsCompletedSuccessfully.Should().BeTrue(because, becauseArgs);
        return new AndConstraint<TaskAssertions>(this);
    }
}
