using Domain.Shared;
using Domain.Users;

namespace Domain.WorkItems;

public sealed class WorkItemComment : Entity
{
    public Guid WorkItemId { get; private set; }
    public WorkItem WorkItem { get; private set; } = null!;
    public Guid AuthorId { get; private set; }
    public User Author { get; private set; } = null!;
    public string Content { get; private set; } = null!;

    public WorkItemComment(Guid workItemId, Guid authorId, string content)
    {
        WorkItemId = workItemId;
        AuthorId = authorId;
        Content = content;
    }

    private WorkItemComment()
    {
    }

    public void UpdateContent(string content)
    {
        Content = content;
    }
}
