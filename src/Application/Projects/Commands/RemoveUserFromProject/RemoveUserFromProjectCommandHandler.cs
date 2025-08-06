using Application.Common.Interfaces.Persistence;
using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Projects.Commands.RemoveUserFromProject;

internal sealed class RemoveUserFromProjectCommandHandler
    : IRequestHandler<RemoveUserFromProjectCommand, ErrorOr<Success>>
{
    private readonly IProjectsRepository _projectsRepository;

    public RemoveUserFromProjectCommandHandler(
        IProjectsRepository projectsRepository)
    {
        _projectsRepository = projectsRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        RemoveUserFromProjectCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdWithUsersAndTeamsAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        var result = project.RemoveUser(command.UserId);
        if (result.IsError)
        {
            return result.Errors;
        }

        _projectsRepository.Update(project);

        return Result.Success;
    }
}