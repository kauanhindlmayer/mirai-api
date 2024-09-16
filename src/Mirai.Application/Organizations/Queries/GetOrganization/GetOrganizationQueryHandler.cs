using ErrorOr;

using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Organizations;

namespace Mirai.Application.Organizations.Queries.GetOrganization;

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
            return OrganizationErrors.OrganizationNotFound;
        }

        return organization;
    }
}