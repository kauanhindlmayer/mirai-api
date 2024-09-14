using Mirai.Domain.Common;
using Mirai.Domain.Projects;
using Mirai.Domain.Users;

namespace Mirai.Domain.WorkItems;

public class WorkItem : Entity
{
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public WorkItemType Type { get; private set; }
    public WorkItemStatus Status { get; private set; }
    public Guid? AssigneeId { get; private set; }
    public User? Assignee { get; private set; }
    public Guid ProjectId { get; private set; }
    public Project Project { get; private set; } = null!;
    public Guid? ParentWorkItemId { get; private set; }
    public WorkItem? ParentWorkItem { get; private set; }
    public ICollection<WorkItem> ChildWorkItems { get; private set; } = [];

    public WorkItem(
        Guid projectId,
        WorkItemType type,
        string title)
    {
        ProjectId = projectId;
        Type = type;
        Title = title;
        Status = WorkItemStatus.New;
    }

    private WorkItem()
    {
    }
}