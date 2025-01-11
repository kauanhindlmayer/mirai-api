using Application.Common.Interfaces.Persistence;
using Domain.Retrospectives;
using Domain.Teams;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Retrospectives.Queries.ListRetrospectives;

internal sealed class ListRetrospectivesQueryHandler
    : IRequestHandler<ListRetrospectivesQuery, ErrorOr<List<RetrospectiveBriefResponse>>>
{
    private readonly IApplicationDbContext _context;

    public ListRetrospectivesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<List<RetrospectiveBriefResponse>>> Handle(
        ListRetrospectivesQuery query,
        CancellationToken cancellationToken)
    {
        var retrospectives = await _context.Retrospectives
            .AsNoTracking()
            .Where(r => r.TeamId == query.TeamId)
            .Select(r => new RetrospectiveBriefResponse
            {
                Id = r.Id,
                Title = r.Title,
            })
            .ToListAsync(cancellationToken);

        return retrospectives;
    }
}
