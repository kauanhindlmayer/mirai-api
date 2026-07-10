using Application.Abstractions;
using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.Organizations.Queries.GetOrganizationUsers;

public sealed record GetOrganizationUsersQuery(
    Guid OrganizationId,
    int Page = 1,
    int PageSize = 10,
    string? Sort = null,
    string? SearchTerm = null,
    Guid? ExcludeProjectId = null) : IAuthorizationRequest<ErrorOr<PaginatedList<OrganizationUserResponse>>>
{
    public Permission RequiredPermission => Permission.OrganizationView;
    public ResourceType ResourceType => ResourceType.Organization;
    public Guid ResourceId => OrganizationId;
}
