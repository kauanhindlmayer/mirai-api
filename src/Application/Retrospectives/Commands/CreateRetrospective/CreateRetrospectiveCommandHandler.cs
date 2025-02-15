using Application.Common.Interfaces.Persistence;
using Domain.Retrospectives;
using Domain.Teams;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.CreateRetrospective;

internal sealed class CreateRetrospectiveCommandHandler
    : IRequestHandler<CreateRetrospectiveCommand, ErrorOr<Guid>>
{
    private readonly ITeamsRepository _teamsRepository;

    public CreateRetrospectiveCommandHandler(ITeamsRepository teamsRepository)
    {
        _teamsRepository = teamsRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(
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
            command.MaxVotesPerUser,
            command.Template,
            command.TeamId);

        var result = team.AddRetrospective(retrospective);
        if (result.IsError)
        {
            return result.Errors;
        }

        _teamsRepository.Update(team);

        return retrospective.Id;
    }
}