namespace Application.Teams.Queries.ListTeams;

public sealed class TeamBriefResponse
{
    public Guid Id { get; init; }
    public Guid BoardId { get; init; }
    public string Name { get; init; } = string.Empty;
}

