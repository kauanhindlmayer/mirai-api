using Domain.WorkItems.Enums;

namespace Presentation.Controllers.WorkItems;

/// <summary>
/// Query parameters for filtering work items.
/// </summary>
public class WorkItemsQueryParameters : PageRequest
{
    /// <summary>
    /// The type of the work item.
    /// </summary>
    public WorkItemType? Type { get; init; }

    /// <summary>
    /// The status of the work item.
    /// </summary>
    public WorkItemStatus? Status { get; init; }

    /// <summary>
    /// The unique identifier of the user who is assigned to the work item.
    /// </summary>
    public Guid? AssigneeId { get; init; }
}