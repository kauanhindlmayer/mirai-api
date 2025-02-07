namespace Contracts.Teams;

public sealed record CreateTeamRequest
{
    /// <summary>
    /// The name of the team.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The description of the team.
    /// </summary>
    public required string Description { get; init; }
}