using Application.Common.Interfaces;
using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Projects.Queries.GetProject;

public class GetProjectQueryHandler(IProjectsRepository _projectsRepository)
    : IRequestHandler<GetProjectQuery, ErrorOr<Project>>
{
    public async Task<ErrorOr<Project>> Handle(
        GetProjectQuery query,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdAsync(
            query.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.ProjectNotFound;
        }

        return project;
    }
}