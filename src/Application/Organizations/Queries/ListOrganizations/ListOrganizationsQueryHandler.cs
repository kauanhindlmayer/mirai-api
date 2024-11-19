using Application.Common.Interfaces.Persistence;
using Domain.Organizations;
using ErrorOr;
using MediatR;

namespace Application.Organizations.Queries.ListOrganizations;

public class ListOrganizationsQueryHandler(IOrganizationsRepository _organizationsRepository)
    : IRequestHandler<ListOrganizationsQuery, ErrorOr<List<Organization>>>
{
    public async Task<ErrorOr<List<Organization>>> Handle(
        ListOrganizationsQuery request,
        CancellationToken cancellationToken)
    {
        return await _organizationsRepository.ListAsync(cancellationToken);
    }
}
