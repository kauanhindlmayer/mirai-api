using Domain.Teams;
using ErrorOr;
using MediatR;

namespace Application.Sprints.Commands.StartSprint;

internal sealed class StartSprintCommandHandler
    : IRequestHandler<StartSprintCommand, ErrorOr<Success>>
{
    private readonly ITeamRepository _teamRepository;

    public StartSprintCommandHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        StartSprintCommand command,
        CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetByIdWithSprintsAsync(
            command.TeamId,
            cancellationToken);

        if (team is null)
        {
            return TeamErrors.NotFound;
        }

        var result = team.StartSprint(command.SprintId);
        if (result.IsError)
        {
            return result.Errors;
        }

        _teamRepository.Update(team);

        return Result.Success;
    }
}
