namespace Application.Teams.Queries.GetTeam;

public sealed class TeamResponse
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public bool IsDefault { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}