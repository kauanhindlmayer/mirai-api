using Domain.Projects;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Projects.Commands.AddUserToProject;

internal sealed class AddUserToProjectCommandHandler
    : IRequestHandler<AddUserToProjectCommand, ErrorOr<Success>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;

    public AddUserToProjectCommandHandler(
        IProjectRepository projectRepository,
        IUserRepository userRepository)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        AddUserToProjectCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdWithOrganizationAndUsersAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        var user = await _userRepository.GetByIdAsync(
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

        _projectRepository.Update(project);

        return Result.Success;
    }
}