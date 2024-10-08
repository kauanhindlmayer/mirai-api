using Application.Common.Interfaces;
using Domain.Retrospectives;
using Domain.Teams;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.CreateRetrospective;

public class CreateRetrospectiveCommandHandler(ITeamsRepository _teamsRepository)
    : IRequestHandler<CreateRetrospectiveCommand, ErrorOr<Retrospective>>
{
    public async Task<ErrorOr<Retrospective>> Handle(
        CreateRetrospectiveCommand command,
        CancellationToken cancellationToken)
    {
        var team = await _teamsRepository.GetByIdAsync(
            command.TeamId,
            cancellationToken);

        if (team is null)
        {
            return TeamErrors.NotFound;
        }

        var retrospective = new Retrospective(
            command.Title,
            command.Description,
            command.TeamId);

        var result = team.AddRetrospective(retrospective);
        if (result.IsError)
        {
            return result.Errors;
        }

        _teamsRepository.Update(team);

        return retrospective;
    }
}