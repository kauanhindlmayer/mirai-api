namespace Application.Sprints.Queries.ListSprints;

public sealed class SprintResponse
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
}