using Domain.WorkItems;
using Domain.WorkItems.Enums;

namespace Domain.UnitTests.WorkItems;

internal static class WorkItemLinkFactory
{
    public const WorkItemLinkType LinkType = WorkItemLinkType.Related;
    public const string Comment = "Link comment";
    public static readonly Guid SourceWorkItemId = Guid.NewGuid();
    public static readonly Guid TargetWorkItemId = Guid.NewGuid();

    public static WorkItemLink Create(
        Guid? sourceWorkItemId = null,
        Guid? targetWorkItemId = null,
        WorkItemLinkType? linkType = null,
        string? comment = null)
    {
        return new WorkItemLink(
            sourceWorkItemId ?? SourceWorkItemId,
            targetWorkItemId ?? TargetWorkItemId,
            linkType ?? LinkType,
            comment ?? Comment);
    }
}
