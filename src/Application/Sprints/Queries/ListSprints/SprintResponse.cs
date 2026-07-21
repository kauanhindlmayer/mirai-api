using Domain.Sprints;

namespace Application.Sprints.Queries.ListSprints;

public sealed class SprintResponse
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public SprintStatus Status { get; init; }
    public DateTime? StartedAtUtc { get; init; }
    public int WorkItemCount { get; init; }
}
