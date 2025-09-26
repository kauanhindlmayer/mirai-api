using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Projects.Commands.RemoveUserFromProject;

internal sealed class RemoveUserFromProjectCommandHandler
    : IRequestHandler<RemoveUserFromProjectCommand, ErrorOr<Success>>
{
    private readonly IProjectRepository _projectRepository;

    public RemoveUserFromProjectCommandHandler(
        IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        RemoveUserFromProjectCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdWithUsersAndTeamsAsync(
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

        _projectRepository.Update(project);

        return Result.Success;
    }
}