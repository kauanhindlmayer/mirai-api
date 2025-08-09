using Domain.Teams;
using Domain.Users;
using ErrorOr;
using MediatR;

namespace Application.Teams.Commands.AddUserToTeam;

internal sealed class AddUserToTeamCommandHandler
    : IRequestHandler<AddUserToTeamCommand, ErrorOr<Success>>
{
    private readonly ITeamsRepository _teamsRepository;
    private readonly IUsersRepository _usersRepository;

    public AddUserToTeamCommandHandler(
        ITeamsRepository teamsRepository,
        IUsersRepository usersRepository)
    {
        _teamsRepository = teamsRepository;
        _usersRepository = usersRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        AddUserToTeamCommand command,
        CancellationToken cancellationToken)
    {
        var team = await _teamsRepository.GetByIdAsync(
            command.TeamId,
            cancellationToken);

        if (team is null)
        {
            return TeamErrors.NotFound;
        }

        var user = await _usersRepository.GetByIdAsync(
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

        _teamsRepository.Update(team);

        return Result.Success;
    }
}