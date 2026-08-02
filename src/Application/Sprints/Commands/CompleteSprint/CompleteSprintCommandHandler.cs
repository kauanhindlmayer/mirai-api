using Domain.Teams;
using ErrorOr;
using MediatR;

namespace Application.Sprints.Commands.CompleteSprint;

internal sealed class CompleteSprintCommandHandler
    : IRequestHandler<CompleteSprintCommand, ErrorOr<Success>>
{
    private readonly ITeamRepository _teamRepository;

    public CompleteSprintCommandHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        CompleteSprintCommand command,
        CancellationToken cancellationToken)
    {
        // Completion moves incomplete items between sprints and freezes the
        // delivered figures from the commitment snapshot, so both the sprints'
        // work items and their commitments must be loaded.
        var team = await _teamRepository.GetByIdWithSprintsWorkItemsAndCommitmentsAsync(
            command.TeamId,
            cancellationToken);

        if (team is null)
        {
            return TeamErrors.NotFound;
        }

        var result = team.CompleteSprint(command.SprintId, command.Dispositions);
        if (result.IsError)
        {
            return result.Errors;
        }

        _teamRepository.Update(team);

        return Result.Success;
    }
}
