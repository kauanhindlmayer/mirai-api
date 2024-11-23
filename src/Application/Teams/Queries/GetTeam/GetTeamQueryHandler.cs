using Application.Common.Interfaces.Persistence;
using Domain.Teams;
using ErrorOr;
using MediatR;

namespace Application.Teams.Queries.GetTeam;

internal sealed class GetTeamQueryHandler(ITeamsRepository teamsRepository)
    : IRequestHandler<GetTeamQuery, ErrorOr<Team>>
{
    public async Task<ErrorOr<Team>> Handle(
        GetTeamQuery query,
        CancellationToken cancellationToken)
    {
        var team = await teamsRepository.GetByIdAsync(
            query.TeamId,
            cancellationToken);

        if (team is null)
        {
            return TeamErrors.NotFound;
        }

        return team;
    }
}