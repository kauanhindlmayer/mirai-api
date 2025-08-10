using Domain.Teams;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Teams.Commands.AddUserToTeam;

internal sealed class AddUserToTeamCommandHandler
    : IRequestHandler<AddUserToTeamCommand, ErrorOr<Success>>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IUserRepository _userRepository;

    public AddUserToTeamCommandHandler(
        ITeamRepository teamRepository,
        IUserRepository userRepository)
    {
        _teamRepository = teamRepository;
        _userRepository = userRepository;
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

        var result = team.AddUser(user);
        if (result.IsError)
        {
            return result.Errors;
        }

        _teamRepository.Update(team);

        return Result.Success;
    }
}