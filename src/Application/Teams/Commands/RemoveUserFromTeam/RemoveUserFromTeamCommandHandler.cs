using Domain.Teams;
using ErrorOr;
using MediatR;

namespace Application.Teams.Commands.RemoveUserFromTeam;

internal sealed class RemoveUserFromTeamCommandHandler
    : IRequestHandler<RemoveUserFromTeamCommand, ErrorOr<Success>>
{
    private readonly ITeamRepository _teamRepository;

    public RemoveUserFromTeamCommandHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        RemoveUserFromTeamCommand command,
        CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetByIdAsync(
            command.TeamId,
            cancellationToken);

        if (team is null)
        {
            return TeamErrors.NotFound;
        }

        var result = team.RemoveUser(command.UserId);
        if (result.IsError)
        {
            return result.Errors;
        }

        _teamRepository.Update(team);

        return Result.Success;
    }
}
