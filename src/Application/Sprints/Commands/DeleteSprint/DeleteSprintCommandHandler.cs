using Domain.Teams;
using ErrorOr;
using MediatR;

namespace Application.Sprints.Commands.DeleteSprint;

internal sealed class DeleteSprintCommandHandler
    : IRequestHandler<DeleteSprintCommand, ErrorOr<Success>>
{
    private readonly ITeamRepository _teamRepository;

    public DeleteSprintCommandHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        DeleteSprintCommand command,
        CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetByIdWithSprintsAndWorkItemsAsync(
            command.TeamId,
            cancellationToken);

        if (team is null)
        {
            return TeamErrors.NotFound;
        }

        var result = team.DeleteSprint(command.SprintId);
        if (result.IsError)
        {
            return result.Errors;
        }

        _teamRepository.Update(team);

        return Result.Success;
    }
}
