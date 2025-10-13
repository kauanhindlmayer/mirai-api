using Domain.WorkItems.Enums;

namespace Presentation.Controllers.WorkItems;

/// <summary>
/// Request to link a work item to another work item.
/// </summary>
/// <param name="TargetWorkItemId">The unique identifier of the target work item to link to.</param>
/// <param name="LinkType">The type of link between the work items.</param>
/// <param name="Comment">An optional comment about the link.</param>
public sealed record LinkWorkItemRequest(
    Guid TargetWorkItemId,
    WorkItemLinkType LinkType,
    string? Comment);