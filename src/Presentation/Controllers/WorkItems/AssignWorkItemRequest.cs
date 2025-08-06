namespace Presentation.Controllers.WorkItems;

/// <summary>
/// Request to assign a work item to a user.
/// </summary>
/// <param name="AssigneeId">
/// The unique identifier of the user to assign the work item to.
/// </param>
public sealed record AssignWorkItemRequest(Guid AssigneeId);