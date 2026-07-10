using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Teams.Commands.ChangeTeamMemberRole;

public sealed record ChangeTeamMemberRoleCommand(
    Guid TeamId,
    Guid UserId,
    Guid RoleId) : IAuthorizationRequest<ErrorOr<Success>>
{
    public Permission RequiredPermission => Permission.TeamManageMembers;
    public ResourceType ResourceType => ResourceType.Team;
    public Guid ResourceId => TeamId;
}
