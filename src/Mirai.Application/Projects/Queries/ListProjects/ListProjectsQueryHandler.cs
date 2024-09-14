using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Projects;

namespace Mirai.Application.Projects.Queries.ListProjects;

public class ListProjectsQueryHandler(IProjectsRepository _projectsRepository)
    : IRequestHandler<ListProjectsQuery, ErrorOr<List<Project>>>
{
    public async Task<ErrorOr<List<Project>>> Handle(
        ListProjectsQuery request,
        CancellationToken cancellationToken)
    {
        return await _projectsRepository.ListAsync(cancellationToken);
    }
}
