using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Projects;

namespace Mirai.Application.Projects.Commands.CreateProject;

public class CreateProjectCommandHandler(IProjectsRepository _projectsRepository)
    : IRequestHandler<CreateProjectCommand, ErrorOr<Project>>
{
    public async Task<ErrorOr<Project>> Handle(
        CreateProjectCommand request,
        CancellationToken cancellationToken)
    {
        var project = new Project(request.Name, request.Description, request.OrganizationId);

        await _projectsRepository.AddAsync(project, cancellationToken);

        return project;
    }
}