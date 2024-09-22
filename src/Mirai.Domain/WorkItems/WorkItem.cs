using ErrorOr;
using Mirai.Domain.Common;
using Mirai.Domain.Projects;
using Mirai.Domain.Users;

namespace Mirai.Domain.WorkItems;

public class WorkItem : Entity
{
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public WorkItemType Type { get; private set; } = null!;
    public WorkItemStatus Status { get; private set; } = null!;
    public Guid? AssigneeId { get; private set; }
    public User? Assignee { get; private set; }
    public Guid ProjectId { get; private set; }
    public Project Project { get; private set; } = null!;
    public Guid? ParentWorkItemId { get; private set; }
    public WorkItem? ParentWorkItem { get; private set; }
    public ICollection<WorkItem> ChildWorkItems { get; private set; } = [];
    public ICollection<Tag> Tags { get; private set; } = [];
    public ICollection<WorkItemComment> Comments { get; private set; } = [];

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

    public void Assign(Guid assigneeId)
    {
        AssigneeId = assigneeId;
    }

    public ErrorOr<Success> AddComment(WorkItemComment comment)
    {
        if (Status == WorkItemStatus.Closed)
        {
            return WorkItemErrors.CannotAddCommentToClosedWorkItem;
        }

        Comments.Add(comment);
        return Result.Success;
    }

    public ErrorOr<Success> RemoveComment(Guid commentId)
    {
        var comment = Comments.SingleOrDefault(c => c.Id == commentId);
        if (comment is null)
        {
            return WorkItemErrors.WorkItemCommentNotFound;
        }

        Comments.Remove(comment);
        return Result.Success;
    }

    public void AddTag(Tag tag)
    {
        Tags.Add(tag);
    }

    public ErrorOr<Success> RemoveTag(Guid tagId)
    {
        var tag = Tags.SingleOrDefault(t => t.Id == tagId);
        if (tag is null)
        {
            return WorkItemErrors.WorkItemTagNotFound;
        }

        Tags.Remove(tag);
        return Result.Success;
    }
}