using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Organizations.Commands.ChangeOrganizationMemberRole;

public sealed record ChangeOrganizationMemberRoleCommand(
    Guid OrganizationId,
    Guid UserId,
    Guid RoleId) : IAuthorizationRequest<ErrorOr<Success>>
{
    public Permission RequiredPermission => Permission.OrganizationManageMembers;
    public ResourceType ResourceType => ResourceType.Organization;
    public Guid ResourceId => OrganizationId;
}
