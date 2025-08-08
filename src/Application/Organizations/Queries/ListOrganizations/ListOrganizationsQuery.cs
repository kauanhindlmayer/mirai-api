using Application.Abstractions.Caching;
using ErrorOr;

namespace Application.Organizations.Queries.ListOrganizations;

public sealed record ListOrganizationsQuery : ICachedQuery<ErrorOr<IReadOnlyList<OrganizationBriefResponse>>>
{
    public string CacheKey => CacheKeys.GetOrganizationsKey();

    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
}
