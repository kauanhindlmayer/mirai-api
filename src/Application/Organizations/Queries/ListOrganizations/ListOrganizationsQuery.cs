using Application.Common;
using Application.Common.Interfaces.Services;
using ErrorOr;

namespace Application.Organizations.Queries.ListOrganizations;

public sealed record ListOrganizationsQuery : ICachedQuery<ErrorOr<List<OrganizationBriefResponse>>>
{
    public string CacheKey => CacheKeys.GetOrganizationsKey();

    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
}
