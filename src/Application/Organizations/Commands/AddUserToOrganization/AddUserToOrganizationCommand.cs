using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Organizations.Commands.AddUserToOrganization;

public sealed record AddUserToOrganizationCommand(
    Guid OrganizationId,
    string Email) : IAuthorizationRequest<ErrorOr<Success>>
{
    public Permission RequiredPermission => Permission.OrganizationManageMembers;
    public ResourceType ResourceType => ResourceType.Organization;
    public Guid ResourceId => OrganizationId;
}
