using Application.Abstractions.Caching;
using ErrorOr;

namespace Application.Organizations.Queries.ListOrganizations;

public sealed record ListOrganizationsQuery(Guid UserId)
    : ICachedQuery<ErrorOr<IReadOnlyList<OrganizationBriefResponse>>>
{
    public string CacheKey => CacheKeys.GetOrganizationsKey(UserId);

    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
}
