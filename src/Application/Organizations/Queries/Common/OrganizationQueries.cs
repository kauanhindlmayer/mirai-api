using System.Linq.Expressions;
using Application.Organizations.Queries.GetOrganization;
using Application.Organizations.Queries.ListOrganizations;
using Domain.Organizations;

namespace Application.Organizations.Queries.Common;

internal static class OrganizationQueries
{
    public static Expression<Func<Organization, OrganizationResponse>> ProjectToDto()
    {
        return o => new OrganizationResponse
        {
            Id = o.Id,
            Name = o.Name,
            Description = o.Description,
            UpdatedAtUtc = o.UpdatedAtUtc,
            CreatedAtUtc = o.CreatedAtUtc,
        };
    }

    public static Expression<Func<Organization, OrganizationBriefResponse>> ProjectToBriefDto()
    {
        return o => new OrganizationBriefResponse
        {
            Id = o.Id,
            Name = o.Name,
        };
    }
}