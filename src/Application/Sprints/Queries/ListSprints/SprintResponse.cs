namespace Application.Sprints.Queries.ListSprints;

public sealed class SprintResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}