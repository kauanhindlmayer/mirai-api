namespace Contracts.Teams;

public sealed record CreateTeamRequest(
    string Name,
    string Description);