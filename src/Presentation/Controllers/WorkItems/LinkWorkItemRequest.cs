using Domain.WorkItems.Enums;

namespace Presentation.Controllers.WorkItems;

public sealed class LinkWorkItemRequest
{
    public required Guid TargetWorkItemId { get; init; }
    public required WorkItemLinkType LinkType { get; init; }
    public string? Comment { get; init; }
}
