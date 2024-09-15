using Mirai.Domain.Common;
using Mirai.Domain.Users;

namespace Mirai.Domain.WorkItems;

public class WorkItemComment : Entity
{
    public Guid WorkItemId { get; private set; }
    public WorkItem WorkItem { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    public string Content { get; private set; } = null!;

    public WorkItemComment(Guid workItemId, Guid userId, string content)
    {
        WorkItemId = workItemId;
        UserId = userId;
        Content = content;
    }

    private WorkItemComment()
    {
    }
}