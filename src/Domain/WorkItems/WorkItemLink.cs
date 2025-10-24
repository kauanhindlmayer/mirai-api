using Domain.Shared;
using Domain.WorkItems.Enums;

namespace Domain.WorkItems;

/// <summary>
/// Represents a directional link between two work items.
/// </summary>
public sealed class WorkItemLink : Entity
{
    public Guid SourceWorkItemId { get; private set; }
    public WorkItem SourceWorkItem { get; private set; } = null!;
    public Guid TargetWorkItemId { get; private set; }
    public WorkItem TargetWorkItem { get; private set; } = null!;
    public WorkItemLinkType LinkType { get; private set; }
    public string? Comment { get; private set; }

    public WorkItemLink(
        Guid sourceWorkItemId,
        Guid targetWorkItemId,
        WorkItemLinkType linkType,
        string? comment = null)
    {
        SourceWorkItemId = sourceWorkItemId;
        TargetWorkItemId = targetWorkItemId;
        LinkType = linkType;
        Comment = comment;
    }

    private WorkItemLink()
    {
    }

    public void UpdateComment(string? comment)
    {
        Comment = comment;
    }
}
