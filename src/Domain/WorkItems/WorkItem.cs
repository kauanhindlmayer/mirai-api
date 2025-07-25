using Domain.Common;
using Domain.Projects;
using Domain.Sprints;
using Domain.Tags;
using Domain.Teams;
using Domain.Users;
using Domain.WorkItems.Enums;
using Domain.WorkItems.ValueObjects;
using ErrorOr;
using Pgvector;

namespace Domain.WorkItems;

public sealed class WorkItem : AggregateRoot
{
    public int Code { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string AcceptanceCriteria { get; private set; } = string.Empty;
    public WorkItemType Type { get; private set; }
    public WorkItemStatus Status { get; private set; }
    public Planning Planning { get; private set; } = new();
    public Classification Classification { get; private set; } = new();
    public Vector SearchVector { get; private set; } = new(new float[384]);
    public Guid? AssigneeId { get; private set; }
    public User? Assignee { get; private set; }
    public Guid ProjectId { get; private set; }
    public Project Project { get; private set; } = null!;
    public Guid? AssignedTeamId { get; private set; }
    public Team AssignedTeam { get; private set; } = null!;
    public Guid? ParentWorkItemId { get; private set; }
    public WorkItem? ParentWorkItem { get; private set; }
    public ICollection<WorkItem> ChildWorkItems { get; private set; } = [];
    public ICollection<Tag> Tags { get; private set; } = [];
    public ICollection<WorkItemComment> Comments { get; private set; } = [];
    public DateTime? CompletedAtUtc { get; private set; }
    public Guid? SprintId { get; private set; }
    public Sprint? Sprint { get; private set; }

    public WorkItem(
        Guid projectId,
        int code,
        string title,
        WorkItemType type,
        Guid? assignedTeamId = null,
        Guid? sprintId = null,
        Guid? parentWorkItemId = null)
    {
        ProjectId = projectId;
        Code = code;
        Title = title;
        Type = type;
        Status = WorkItemStatus.New;
        AssignedTeamId = assignedTeamId;
        SprintId = sprintId;
        ParentWorkItemId = parentWorkItemId;
    }

    private WorkItem()
    {
    }

    public void Assign(Guid assigneeId)
    {
        AssigneeId = assigneeId;
    }

    public void Close()
    {
        Status = WorkItemStatus.Closed;
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
            return WorkItemErrors.CommentNotFound;
        }

        Comments.Remove(comment);
        return Result.Success;
    }

    public void AddTag(Tag tag)
    {
        Tags.Add(tag);
    }

    public ErrorOr<Success> RemoveTag(string tagName)
    {
        var tag = Tags.SingleOrDefault(t => t.Name == tagName);
        if (tag is null)
        {
            return TagErrors.NotFound;
        }

        Tags.Remove(tag);
        return Result.Success;
    }

    public void SetSearchVector(float[] embedding)
    {
        SearchVector = new Vector(embedding);
    }
}