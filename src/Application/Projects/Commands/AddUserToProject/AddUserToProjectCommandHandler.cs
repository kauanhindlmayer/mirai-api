using Domain.Organizations;
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
    private readonly IOrganizationRepository _organizationRepository;

    public AddUserToProjectCommandHandler(
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        IOrganizationRepository organizationRepository)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _organizationRepository = organizationRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        AddUserToProjectCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdWithUsersAndTeamsAsync(
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

        if (!await _organizationRepository.IsUserInOrganizationAsync(
            project.OrganizationId,
            user.Id,
            cancellationToken))
        {
            return ProjectErrors.UserNotInOrganization;
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