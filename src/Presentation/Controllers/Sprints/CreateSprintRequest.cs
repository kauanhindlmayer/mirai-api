namespace Presentation.Controllers.Sprints;

/// <summary>
/// Data transfer object for creating a sprint.
/// </summary>
/// <param name="Name">The name of the sprint.</param>
/// <param name="StartDate">The start date of the sprint.</param>
/// <param name="EndDate">The end date of the sprint.</param>
public sealed record CreateSprintRequest(
    string Name,
    DateOnly StartDate,
    DateOnly EndDate);