using Application.Common.Interfaces.Persistence;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Teams.Queries.ListTeams;

internal sealed class ListTeamsQueryHandler
    : IRequestHandler<ListTeamsQuery, ErrorOr<IReadOnlyList<TeamBriefResponse>>>
{
    private readonly IApplicationDbContext _context;

    public ListTeamsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<IReadOnlyList<TeamBriefResponse>>> Handle(
        ListTeamsQuery query,
        CancellationToken cancellationToken)
    {
        var teams = await _context.Teams
            .AsNoTracking()
            .Where(t => t.ProjectId == query.ProjectId)
            .Select(t => new TeamBriefResponse
            {
                Id = t.Id,
                Name = t.Name,
                BoardId = t.Board.Id,
            })
            .ToListAsync(cancellationToken);

        return teams;
    }
}