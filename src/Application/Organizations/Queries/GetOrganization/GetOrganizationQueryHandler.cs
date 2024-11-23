using Application.Common.Interfaces.Persistence;
using Domain.Organizations;
using ErrorOr;
using MediatR;

namespace Application.Organizations.Queries.GetOrganization;

internal sealed class GetOrganizationQueryHandler(
    IOrganizationsRepository organizationsRepository)
    : IRequestHandler<GetOrganizationQuery, ErrorOr<Organization>>
{
    public async Task<ErrorOr<Organization>> Handle(
        GetOrganizationQuery query,
        CancellationToken cancellationToken)
    {
        var organization = await organizationsRepository.GetByIdAsync(
            query.OrganizationId,
            cancellationToken);

        if (organization is null)
        {
            return OrganizationErrors.NotFound;
        }

        return organization;
    }
}