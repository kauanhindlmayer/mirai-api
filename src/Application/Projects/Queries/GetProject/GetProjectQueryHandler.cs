using Application.Common.Interfaces.Persistence;
using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Projects.Queries.GetProject;

internal sealed class GetProjectQueryHandler(IProjectsRepository projectsRepository)
    : IRequestHandler<GetProjectQuery, ErrorOr<Project>>
{
    public async Task<ErrorOr<Project>> Handle(
        GetProjectQuery query,
        CancellationToken cancellationToken)
    {
        var project = await projectsRepository.GetByIdAsync(
            query.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        return project;
    }
}