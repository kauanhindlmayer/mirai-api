using Application.Common.Interfaces.Persistence;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Retrospectives.Queries.ListRetrospectives;

internal sealed class ListRetrospectivesQueryHandler
    : IRequestHandler<ListRetrospectivesQuery, ErrorOr<IReadOnlyList<RetrospectiveBriefResponse>>>
{
    private readonly IApplicationDbContext _context;

    public ListRetrospectivesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<IReadOnlyList<RetrospectiveBriefResponse>>> Handle(
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
