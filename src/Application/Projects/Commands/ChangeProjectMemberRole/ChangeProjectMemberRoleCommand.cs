using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Projects.Commands.ChangeProjectMemberRole;

public sealed record ChangeProjectMemberRoleCommand(
    Guid ProjectId,
    Guid UserId,
    Guid RoleId) : IAuthorizationRequest<ErrorOr<Success>>
{
    public Permission RequiredPermission => Permission.ProjectManageMembers;
    public ResourceType ResourceType => ResourceType.Project;
    public Guid ResourceId => ProjectId;
}
