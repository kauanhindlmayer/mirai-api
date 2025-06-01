namespace Presentation.Controllers.Sprints;

/// <summary>
/// Data transfer object for adding a work item to a sprint.
/// </summary>
/// <param name="WorkItemId">The work item's unique identifier.</param>
public sealed record AddWorkItemToSprintRequest(Guid WorkItemId);