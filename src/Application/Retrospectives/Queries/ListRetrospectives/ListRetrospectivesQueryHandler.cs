using Application.Common.Interfaces.Persistence;
using Domain.Retrospectives;
using Domain.Teams;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Queries.ListRetrospectives;

internal sealed class ListRetrospectivesQueryHandler(ITeamsRepository teamsRepository)
    : IRequestHandler<ListRetrospectivesQuery, ErrorOr<List<Retrospective>>>
{
    public async Task<ErrorOr<List<Retrospective>>> Handle(
        ListRetrospectivesQuery query,
        CancellationToken cancellationToken)
    {
        var team = await teamsRepository.GetByIdWithRetrospectivesAsync(
            query.TeamId,
            cancellationToken);

        if (team is null)
        {
            return TeamErrors.NotFound;
        }

        return team.Retrospectives.ToList();
    }
}
