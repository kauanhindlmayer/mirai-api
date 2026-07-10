using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Organizations.Commands.DeleteOrganization;

public sealed record DeleteOrganizationCommand(Guid OrganizationId)
    : IAuthorizationRequest<ErrorOr<Success>>
{
    public Permission RequiredPermission => Permission.OrganizationDelete;
    public ResourceType ResourceType => ResourceType.Organization;
    public Guid ResourceId => OrganizationId;
}
