using Application.Abstractions;
using Domain.Authorization;
using Domain.Projects;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Commands.ChangeProjectMemberRole;

internal sealed class ChangeProjectMemberRoleCommandHandler
    : IRequestHandler<ChangeProjectMemberRoleCommand, ErrorOr<Success>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IApplicationDbContext _context;

    public ChangeProjectMemberRoleCommandHandler(
        IProjectRepository projectRepository,
        IApplicationDbContext context)
    {
        _projectRepository = projectRepository;
        _context = context;
    }

    public async Task<ErrorOr<Success>> Handle(
        ChangeProjectMemberRoleCommand command,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdWithUsersAndTeamsAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return ProjectErrors.NotFound;
        }

        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == command.RoleId, cancellationToken);

        if (role is null)
        {
            return RoleErrors.NotFound;
        }

        var result = project.ChangeMemberRole(command.UserId, role);
        if (result.IsError)
        {
            return result.Errors;
        }

        _projectRepository.Update(project);

        return Result.Success;
    }
}
