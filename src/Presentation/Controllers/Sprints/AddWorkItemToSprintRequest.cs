namespace Presentation.Controllers.Sprints;

/// <summary>
/// Request to add a work item to a sprint.
/// </summary>
/// <param name="WorkItemId">The work item's unique identifier.</param>
public sealed record AddWorkItemToSprintRequest(Guid WorkItemId);