using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Teams.Commands.AddUserToTeam;

public sealed record AddUserToTeamCommand(
    Guid TeamId,
    Guid UserId) : IAuthorizationRequest<ErrorOr<Success>>
{
    public Permission RequiredPermission => Permission.TeamManageMembers;
    public ResourceType ResourceType => ResourceType.Team;
    public Guid ResourceId => TeamId;
}
