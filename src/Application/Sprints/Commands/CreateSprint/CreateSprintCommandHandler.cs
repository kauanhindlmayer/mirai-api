using Domain.Sprints;
using Domain.Teams;
using ErrorOr;
using MediatR;

namespace Application.Sprints.Commands.CreateSprint;

internal sealed class CreateSprintCommandHandler
    : IRequestHandler<CreateSprintCommand, ErrorOr<Guid>>
{
    private readonly ITeamRepository _teamRepository;

    public CreateSprintCommandHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(
        CreateSprintCommand command,
        CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetByIdAsync(
            command.TeamId,
            cancellationToken);

        if (team is null)
        {
            return TeamErrors.NotFound;
        }

        var sprint = new Sprint(
            command.TeamId,
            command.Name,
            command.StartDate,
            command.EndDate);

        var result = team.AddSprint(sprint);
        if (result.IsError)
        {
            return result.Errors;
        }

        _teamRepository.Update(team);

        return sprint.Id;
    }
}