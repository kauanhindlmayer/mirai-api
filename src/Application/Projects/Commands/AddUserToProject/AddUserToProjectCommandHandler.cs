using Application.Abstractions;
using Domain.Authorization;
using Domain.Organizations;
using Domain.Projects;
using Domain.Users;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Commands.AddUserToProject;

internal sealed class AddUserToProjectCommandHandler
    : IRequestHandler<AddUserToProjectCommand, ErrorOr<Success>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IApplicationDbContext _context;

    public AddUserToProjectCommandHandler(
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        IOrganizationRepository organizationRepository,
        IApplicationDbContext context)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _organizationRepository = organizationRepository;
        _context = context;
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

        if (project.Members.Any(m => m.UserId == user.Id))
        {
            return ProjectErrors.UserAlreadyExists;
        }

        var contributorRole = await _context.Roles
            .FirstAsync(r => r.Id == SystemRoles.ProjectContributorId, cancellationToken);

        var result = project.AddMember(user, contributorRole);
        if (result.IsError)
        {
            return result.Errors;
        }

        _projectRepository.Update(project);

        return Result.Success;
    }
}