using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Organizations.Commands.UpdateOrganization;

public sealed record UpdateOrganizationCommand(
    Guid Id,
    string Name,
    string Description) : IAuthorizationRequest<ErrorOr<Guid>>
{
    public Permission RequiredPermission => Permission.OrganizationManage;
    public ResourceType ResourceType => ResourceType.Organization;
    public Guid ResourceId => Id;
}
