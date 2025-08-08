using Domain.Sprints;
using Domain.Teams;
using ErrorOr;
using MediatR;

namespace Application.Sprints.Commands.CreateSprint;

internal sealed class CreateSprintCommandHandler
    : IRequestHandler<CreateSprintCommand, ErrorOr<Guid>>
{
    private readonly ITeamsRepository _teamsRepository;

    public CreateSprintCommandHandler(ITeamsRepository teamsRepository)
    {
        _teamsRepository = teamsRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(
        CreateSprintCommand command,
        CancellationToken cancellationToken)
    {
        var team = await _teamsRepository.GetByIdAsync(
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

        _teamsRepository.Update(team);

        return sprint.Id;
    }
}