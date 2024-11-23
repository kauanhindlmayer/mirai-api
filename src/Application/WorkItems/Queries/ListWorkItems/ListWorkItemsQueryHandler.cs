using Application.Common.Interfaces.Persistence;
using Domain.Projects;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Queries.ListWorkItems;

internal sealed class ListWorkItemsQueryHandler(IProjectsRepository projectsRepository)
    : IRequestHandler<ListWorkItemsQuery, ErrorOr<List<WorkItem>>>
{
    public async Task<ErrorOr<List<WorkItem>>> Handle(
        ListWorkItemsQuery query,
        CancellationToken cancellationToken)
    {
        var project = await projectsRepository.GetByIdWithWorkItemsAsync(
            query.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        return project.WorkItems.ToList();
    }
}