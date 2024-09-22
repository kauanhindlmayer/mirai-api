using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Retrospectives;
using Mirai.Domain.Teams;

namespace Mirai.Application.Retrospectives.Commands.CreateRetrospective;

public class CreateRetrospectiveCommandHandler(
    ITeamsRepository _teamsRepository,
    IRetrospectivesRepository _retrospectivesRepository)
    : IRequestHandler<CreateRetrospectiveCommand, ErrorOr<Retrospective>>
{
    public async Task<ErrorOr<Retrospective>> Handle(
        CreateRetrospectiveCommand request,
        CancellationToken cancellationToken)
    {
        var team = await _teamsRepository.GetByIdAsync(
            request.TeamId,
            cancellationToken);

        if (team is null)
        {
            return TeamErrors.TeamNotFound;
        }

        var retrospective = new Retrospective(
            request.Title,
            request.Description,
            request.TeamId);

        await _retrospectivesRepository.AddAsync(retrospective, cancellationToken);

        return retrospective;
    }
}