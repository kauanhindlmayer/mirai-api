using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Organizations;

namespace Mirai.Application.Organizations.Queries.ListOrganizations;

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
