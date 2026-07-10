using Application.Abstractions;
using Domain.Authorization;
using Domain.Teams;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Teams.Commands.ChangeTeamMemberRole;

internal sealed class ChangeTeamMemberRoleCommandHandler
    : IRequestHandler<ChangeTeamMemberRoleCommand, ErrorOr<Success>>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IApplicationDbContext _context;

    public ChangeTeamMemberRoleCommandHandler(
        ITeamRepository teamRepository,
        IApplicationDbContext context)
    {
        _teamRepository = teamRepository;
        _context = context;
    }

    public async Task<ErrorOr<Success>> Handle(
        ChangeTeamMemberRoleCommand command,
        CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetByIdAsync(
            command.TeamId,
            cancellationToken);

        if (team is null)
        {
            return TeamErrors.NotFound;
        }

        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == command.RoleId, cancellationToken);

        if (role is null)
        {
            return RoleErrors.NotFound;
        }

        var result = team.ChangeMemberRole(command.UserId, role);
        if (result.IsError)
        {
            return result.Errors;
        }

        _teamRepository.Update(team);

        return Result.Success;
    }
}
