using Application.Abstractions;
using Domain.Authorization;
using Domain.Teams;
using Domain.Users;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Teams.Commands.AddUserToTeam;

internal sealed class AddUserToTeamCommandHandler
    : IRequestHandler<AddUserToTeamCommand, ErrorOr<Success>>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IUserRepository _userRepository;
    private readonly IApplicationDbContext _context;

    public AddUserToTeamCommandHandler(
        ITeamRepository teamRepository,
        IUserRepository userRepository,
        IApplicationDbContext context)
    {
        _teamRepository = teamRepository;
        _userRepository = userRepository;
        _context = context;
    }

    public async Task<ErrorOr<Success>> Handle(
        AddUserToTeamCommand command,
        CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetByIdAsync(
            command.TeamId,
            cancellationToken);

        if (team is null)
        {
            return TeamErrors.NotFound;
        }

        var user = await _userRepository.GetByIdAsync(
            command.UserId,
            cancellationToken);

        if (user is null)
        {
            return TeamErrors.UserNotFound;
        }

        if (team.Members.Any(m => m.UserId == user.Id))
        {
            return TeamErrors.UserAlreadyExists;
        }

        var memberRole = await _context.Roles
            .FirstAsync(r => r.Id == SystemRoles.TeamMemberId, cancellationToken);

        var result = team.AddMember(user, memberRole);
        if (result.IsError)
        {
            return result.Errors;
        }

        _teamRepository.Update(team);

        return Result.Success;
    }
}