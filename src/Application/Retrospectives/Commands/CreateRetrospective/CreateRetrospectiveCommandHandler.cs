using Application.Common.Interfaces.Persistence;
using Domain.Retrospectives;
using Domain.Teams;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.CreateRetrospective;

internal sealed class CreateRetrospectiveCommandHandler(ITeamsRepository teamsRepository)
    : IRequestHandler<CreateRetrospectiveCommand, ErrorOr<Retrospective>>
{
    public async Task<ErrorOr<Retrospective>> Handle(
        CreateRetrospectiveCommand command,
        CancellationToken cancellationToken)
    {
        var team = await teamsRepository.GetByIdAsync(
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

        teamsRepository.Update(team);

        return retrospective;
    }
}