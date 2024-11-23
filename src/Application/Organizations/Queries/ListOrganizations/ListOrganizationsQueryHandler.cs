using Application.Common.Interfaces.Persistence;
using Domain.Organizations;
using ErrorOr;
using MediatR;

namespace Application.Organizations.Queries.ListOrganizations;

internal sealed class ListOrganizationsQueryHandler(
    IOrganizationsRepository organizationsRepository)
    : IRequestHandler<ListOrganizationsQuery, ErrorOr<List<Organization>>>
{
    public async Task<ErrorOr<List<Organization>>> Handle(
        ListOrganizationsQuery request,
        CancellationToken cancellationToken)
    {
        return await organizationsRepository.ListAsync(cancellationToken);
    }
}
