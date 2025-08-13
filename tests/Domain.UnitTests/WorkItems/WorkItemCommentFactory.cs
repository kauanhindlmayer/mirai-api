using Domain.WorkItems;

namespace Domain.UnitTests.WorkItems;

internal static class WorkItemCommentFactory
{
    public const string Content = "Comment Content";
    public static readonly Guid WorkItemId = Guid.NewGuid();
    public static readonly Guid AuthorId = Guid.NewGuid();

    public static WorkItemComment Create(
        Guid? workItemId = null,
        Guid? authorId = null,
        string? content = null)
    {
        return new(
            workItemId ?? WorkItemId,
            authorId ?? AuthorId,
            content ?? Content);
    }
}