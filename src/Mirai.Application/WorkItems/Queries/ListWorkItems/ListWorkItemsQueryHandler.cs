using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Projects;
using Mirai.Domain.WorkItems;

namespace Mirai.Application.WorkItems.Queries.ListWorkItems;

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