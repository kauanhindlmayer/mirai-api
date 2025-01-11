namespace Application.Teams.Queries.GetTeam;

public sealed class TeamResponse
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }
    public string Name { get; init; } = string.Empty;
    public IEnumerable<MemberResponse> Members { get; init; } = [];
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public sealed class MemberResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}