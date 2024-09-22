namespace Mirai.Contracts.Teams;

public record CreateTeamRequest(
    Guid ProjectId,
    string Name);