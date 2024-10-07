using Application.Common.Interfaces;
using Domain.Organizations;
using ErrorOr;
using MediatR;

namespace Application.Organizations.Queries.GetOrganization;

public class GetOrganizationQueryHandler(IOrganizationsRepository _organizationsRepository)
    : IRequestHandler<GetOrganizationQuery, ErrorOr<Organization>>
{
    public async Task<ErrorOr<Organization>> Handle(
        GetOrganizationQuery query,
        CancellationToken cancellationToken)
    {
        var organization = await _organizationsRepository.GetByIdAsync(
            query.OrganizationId,
            cancellationToken);

        if (organization is null)
        {
            return OrganizationErrors.NotFound;
        }

        return organization;
    }
}