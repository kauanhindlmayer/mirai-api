using Application.Common.Interfaces.Persistence;
using Domain.Projects;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Projects.Commands.AddUserToProject;

internal sealed class AddUserToProjectCommandHandler
    : IRequestHandler<AddUserToProjectCommand, ErrorOr<Success>>
{
    private readonly IProjectsRepository _projectsRepository;
    private readonly IUsersRepository _usersRepository;

    public AddUserToProjectCommandHandler(
        IProjectsRepository projectsRepository,
        IUsersRepository usersRepository)
    {
        _projectsRepository = projectsRepository;
        _usersRepository = usersRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        AddUserToProjectCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetByIdWithOrganizationAndUsersAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        var user = await _usersRepository.GetByIdAsync(
            command.UserId,
            cancellationToken);

        if (user is null)
        {
            return UserErrors.NotFound;
        }

        var result = project.AddUser(user);
        if (result.IsError)
        {
            return result.Errors;
        }

        _projectsRepository.Update(project);

        return Result.Success;
    }
}