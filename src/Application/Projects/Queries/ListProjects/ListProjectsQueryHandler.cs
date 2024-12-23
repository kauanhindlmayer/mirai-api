using Application.Common.Interfaces.Persistence;
using Domain.Organizations;
using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Projects.Queries.ListProjects;

internal sealed class ListProjectsQueryHandler(IOrganizationsRepository organizationsRepository)
    : IRequestHandler<ListProjectsQuery, ErrorOr<List<Project>>>
{
    public async Task<ErrorOr<List<Project>>> Handle(
        ListProjectsQuery query,
        CancellationToken cancellationToken)
    {
        var organization = await organizationsRepository.GetByIdWithProjectsAsync(
            query.OrganizationId,
            cancellationToken);

        if (organization is null)
        {
            return OrganizationErrors.NotFound;
        }

        return organization.Projects.ToList();
    }
}
