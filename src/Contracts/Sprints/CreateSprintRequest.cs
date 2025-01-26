namespace Contracts.Sprints;

public sealed record CreateSprintRequest(
    string Name,
    DateTime StartDate,
    DateTime EndDate);