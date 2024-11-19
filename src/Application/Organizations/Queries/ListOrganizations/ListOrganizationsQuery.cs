using Application.Common.Caching;
using Domain.Organizations;
using ErrorOr;

namespace Application.Organizations.Queries.ListOrganizations;

public record ListOrganizationsQuery() : ICachedQuery<ErrorOr<List<Organization>>>
{
    public string CacheKey => CacheKeys.GetOrganizationsKey();

    public TimeSpan? Expiration => null;
}
