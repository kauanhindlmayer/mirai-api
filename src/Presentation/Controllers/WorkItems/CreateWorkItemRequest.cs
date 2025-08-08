using Domain.WorkItems.Enums;

namespace Presentation.Controllers.WorkItems;

/// <summary>
/// Request to create a new work item.
/// </summary>
/// <param name="Type">The type of the work item.</param>
/// <param name="Title">The title of the work item.</param>
/// <param name="AssignedTeamId">
/// The unique identifier of the team to assign the work item to.
/// </param>
public sealed record CreateWorkItemRequest(
    WorkItemType Type,
    string Title,
    Guid AssignedTeamId);