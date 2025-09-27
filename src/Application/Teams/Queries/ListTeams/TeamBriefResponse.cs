namespace Application.Teams.Queries.ListTeams;

public sealed class TeamBriefResponse
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public Guid BoardId { get; init; }
    public bool IsDefault { get; init; }
}