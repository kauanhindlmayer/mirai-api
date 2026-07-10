using Application.Abstractions.Authorization;
using Application.Abstractions.Caching;
using Domain.Authorization;
using ErrorOr;

namespace Application.Organizations.Queries.GetOrganization;

public sealed record GetOrganizationQuery(Guid OrganizationId)
    : ICachedQuery<ErrorOr<OrganizationResponse>>, IAuthorizationRequest<ErrorOr<OrganizationResponse>>
{
    public string CacheKey => CacheKeys.GetOrganizationKey(OrganizationId);

    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);

    public Permission RequiredPermission => Permission.OrganizationView;

    public ResourceType ResourceType => ResourceType.Organization;

    public Guid ResourceId => OrganizationId;
}
