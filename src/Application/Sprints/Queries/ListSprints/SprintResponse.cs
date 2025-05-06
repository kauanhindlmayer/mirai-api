namespace Application.Sprints.Queries.ListSprints;

public sealed class SprintResponse
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}