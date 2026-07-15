namespace Presentation.Controllers.Sprints;

/// <summary>
/// Request to update an existing sprint.
/// </summary>
/// <param name="Name">The new name of the sprint.</param>
/// <param name="StartDate">The new start date of the sprint.</param>
/// <param name="EndDate">The new end date of the sprint.</param>
public sealed record UpdateSprintRequest(
    string Name,
    DateOnly StartDate,
    DateOnly EndDate);
