using Application.Common.Interfaces;
using Domain.Organizations;
using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Projects.Queries.ListProjects;

public class ListProjectsQueryHandler(IOrganizationsRepository _organizationsRepository)
    : IRequestHandler<ListProjectsQuery, ErrorOr<List<Project>>>
{
    public async Task<ErrorOr<List<Project>>> Handle(
        ListProjectsQuery query,
        CancellationToken cancellationToken)
    {
        var organization = await _organizationsRepository.GetByIdWithProjectsAsync(
            query.OrganizationId,
            cancellationToken);

        if (organization is null)
        {
            return OrganizationErrors.OrganizationNotFound;
        }

        return organization.Projects.ToList();
    }
}
