using Application.Common.Interfaces;
using Domain.Projects;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Queries.ListWorkItems;

public class ListWorkItemsQueryHandler(IProjectsRepository _projectsRepository)
    : IRequestHandler<ListWorkItemsQuery, ErrorOr<List<WorkItem>>>
{
    public async Task<ErrorOr<List<WorkItem>>> Handle(
        ListWorkItemsQuery query,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdWithWorkItemsAsync(
            query.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.ProjectNotFound;
        }

        return project.WorkItems.ToList();
    }
}