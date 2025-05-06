using Application.Common.Interfaces.Persistence;
using Application.WikiPages.Common;
using Domain.WikiPages;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.WikiPages.Queries.ListWikiPages;

internal sealed class ListWikiPagesQueryHandler
    : IRequestHandler<ListWikiPagesQuery, ErrorOr<IReadOnlyList<WikiPageBriefResponse>>>
{
    private readonly IApplicationDbContext _context;

    public ListWikiPagesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<IReadOnlyList<WikiPageBriefResponse>>> Handle(
        ListWikiPagesQuery query,
        CancellationToken cancellationToken)
    {
        var rootPages = await _context.WikiPages
            .AsNoTracking()
            .Include(wp => wp.SubWikiPages)
            .Where(wp =>
                wp.ProjectId == query.ProjectId &&
                !wp.ParentWikiPageId.HasValue)
            .OrderBy(wp => wp.Position)
            .Select(WikiPageQueries.ProjectToBriefDto())
            .ToListAsync(cancellationToken);

        return rootPages;
    }
}
