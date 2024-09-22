using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Teams;

namespace Mirai.Application.Teams.Queries.GetTeam;

public class GetTeamQueryHandler(ITeamsRepository _teamsRepository)
    : IRequestHandler<GetTeamQuery, ErrorOr<Team>>
{
    public async Task<ErrorOr<Team>> Handle(
        GetTeamQuery query,
        CancellationToken cancellationToken)
    {
        var team = await _teamsRepository.GetByIdAsync(
            query.TeamId,
            cancellationToken);

        if (team is null)
        {
            return TeamErrors.TeamNotFound;
        }

        return team;
    }
}