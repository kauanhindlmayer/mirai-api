using Application.Common.Interfaces.Persistence;
using Domain.Retrospectives;
using Domain.Teams;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Queries.ListRetrospectives;

public class ListRetrospectivesQueryHandler(ITeamsRepository _teamsRepository)
    : IRequestHandler<ListRetrospectivesQuery, ErrorOr<List<Retrospective>>>
{
    public async Task<ErrorOr<List<Retrospective>>> Handle(
        ListRetrospectivesQuery query,
        CancellationToken cancellationToken)
    {
        var team = await _teamsRepository.GetByIdWithRetrospectivesAsync(
            query.TeamId,
            cancellationToken);

        if (team is null)
        {
            return TeamErrors.NotFound;
        }

        return team.Retrospectives.ToList();
    }
}
