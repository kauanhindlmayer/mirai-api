using Application.Abstractions;
using Domain.Teams;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Teams.Queries.GetTeam;

internal sealed class GetTeamQueryHandler
    : IRequestHandler<GetTeamQuery, ErrorOr<TeamResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetTeamQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<TeamResponse>> Handle(
        GetTeamQuery query,
        CancellationToken cancellationToken)
    {
        var team = await _context.Teams
            .AsNoTracking()
            .Where(t => t.Id == query.TeamId)
            .Select(TeamQueries.ProjectToDto())
            .FirstOrDefaultAsync(cancellationToken);

        if (team is null)
        {
            return TeamErrors.NotFound;
        }

        return team;
    }
}