using Application.Abstractions.Caching;
using ErrorOr;

namespace Application.Organizations.Queries.GetOrganization;

public sealed record GetOrganizationQuery(Guid OrganizationId)
    : ICachedQuery<ErrorOr<OrganizationResponse>>
{
    public string CacheKey => CacheKeys.GetOrganizationKey(OrganizationId);

    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
}
