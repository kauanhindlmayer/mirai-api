using Application.Common.Interfaces.Persistence;
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
            .Select(wp => ToDto(wp))
            .ToListAsync(cancellationToken);

        return rootPages;
    }

    private static WikiPageBriefResponse ToDto(WikiPage wikiPage)
    {
        return new WikiPageBriefResponse
        {
            Id = wikiPage.Id,
            Title = wikiPage.Title,
            Position = wikiPage.Position,
            SubPages = wikiPage.SubWikiPages.Select(ToDto),
        };
    }
}
