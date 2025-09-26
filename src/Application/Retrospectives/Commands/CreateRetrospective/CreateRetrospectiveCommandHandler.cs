using Domain.Retrospectives;
using Domain.Teams;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.CreateRetrospective;

internal sealed class CreateRetrospectiveCommandHandler
    : IRequestHandler<CreateRetrospectiveCommand, ErrorOr<Guid>>
{
    private readonly ITeamRepository _teamRepository;

    public CreateRetrospectiveCommandHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(
        CreateRetrospectiveCommand command,
        CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetByIdAsync(
            command.TeamId,
            cancellationToken);

        if (team is null)
        {
            return TeamErrors.NotFound;
        }

        var retrospective = new Retrospective(
            command.Title,
            command.TeamId,
            command.MaxVotesPerUser,
            command.Template);

        var result = team.AddRetrospective(retrospective);
        if (result.IsError)
        {
            return result.Errors;
        }

        _teamRepository.Update(team);

        return retrospective.Id;
    }
}