using Application.Abstractions.Authentication;
using Domain.Organizations;
using Domain.Projects;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Projects.Commands.CreateProject;

internal sealed class CreateProjectCommandHandler
    : IRequestHandler<CreateProjectCommand, ErrorOr<Guid>>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;

    public CreateProjectCommandHandler(
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository,
        IUserContext userContext)
    {
        _organizationRepository = organizationRepository;
        _userRepository = userRepository;
        _userContext = userContext;
    }

    public async Task<ErrorOr<Guid>> Handle(
        CreateProjectCommand command,
        CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetByIdWithProjectsAsync(
            command.OrganizationId,
            cancellationToken);

        if (organization is null)
        {
            return OrganizationErrors.NotFound;
        }

        var currentUser = await _userRepository.GetByIdAsync(
            _userContext.UserId,
            cancellationToken);

        if (currentUser is null)
        {
            return UserErrors.NotFound;
        }

        if (!await _organizationRepository.IsUserInOrganizationAsync(
            command.OrganizationId,
            currentUser.Id,
            cancellationToken))
        {
            return ProjectErrors.UserNotInOrganization;
        }

        var project = new Project(
            command.Name,
            command.Description,
            command.OrganizationId);

        var addProjectResult = organization.AddProject(project);
        if (addProjectResult.IsError)
        {
            return addProjectResult.Errors;
        }

        var addUserResult = project.AddUser(currentUser);
        if (addUserResult.IsError)
        {
            return addUserResult.Errors;
        }

        _organizationRepository.Update(organization);

        return project.Id;
    }
}