using Domain.Teams;
using ErrorOr;
using MediatR;

namespace Application.Sprints.Commands.UpdateSprint;

internal sealed class UpdateSprintCommandHandler
    : IRequestHandler<UpdateSprintCommand, ErrorOr<Success>>
{
    private readonly ITeamRepository _teamRepository;

    public UpdateSprintCommandHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        UpdateSprintCommand command,
        CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetByIdWithSprintsAsync(
            command.TeamId,
            cancellationToken);

        if (team is null)
        {
            return TeamErrors.NotFound;
        }

        var result = team.UpdateSprint(
            command.SprintId,
            command.Name,
            command.StartDate,
            command.EndDate);

        if (result.IsError)
        {
            return result.Errors;
        }

        _teamRepository.Update(team);

        return Result.Success;
    }
}
