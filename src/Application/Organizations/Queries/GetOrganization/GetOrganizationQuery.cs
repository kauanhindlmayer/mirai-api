using Application.Common;
using Application.Common.Interfaces.Services;
using Domain.Organizations;
using ErrorOr;

namespace Application.Organizations.Queries.GetOrganization;

public sealed record GetOrganizationQuery(Guid OrganizationId)
    : ICachedQuery<ErrorOr<Organization>>
{
    public string CacheKey => CacheKeys.GetOrganizationKey(OrganizationId);

    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
}
