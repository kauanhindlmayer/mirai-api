using Application.Common.Interfaces.Persistence;
using Domain.Teams;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Teams.Queries.ListTeams;

internal sealed class ListTeamsQueryHandler : IRequestHandler<ListTeamsQuery, ErrorOr<TeamBriefResponse>>
{
    private readonly IApplicationDbContext _context;

    public ListTeamsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<TeamBriefResponse>> Handle(
        ListTeamsQuery query,
        CancellationToken cancellationToken)
    {
        var team = await _context.Teams
            .AsNoTracking()
            .Where(t => t.ProjectId == query.ProjectId)
            .Select(t => new TeamBriefResponse
            {
                Id = t.Id,
                BoardId = t.Board.Id,
                Name = t.Name,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (team is null)
        {
            return TeamErrors.NotFound;
        }

        return team;
    }
}