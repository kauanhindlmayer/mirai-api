using Application.Common;
using Application.Common.Interfaces.Services;
using Domain.Organizations;
using ErrorOr;

namespace Application.Organizations.Queries.ListOrganizations;

public record ListOrganizationsQuery() : ICachedQuery<ErrorOr<List<Organization>>>
{
    public string CacheKey => CacheKeys.GetOrganizationsKey();

    public TimeSpan? Expiration => null;
}
