using Application.Common.Interfaces.Persistence;
using Domain.Projects;
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
            .Where(wp => wp.ProjectId == query.ProjectId &&
                         wp.ParentWikiPageId == null)
            .OrderBy(wp => wp.Position)
            .ToListAsync(cancellationToken);

        // Mapping client-side due to non-translatable hierarchical transformations.
        return rootPages.ConvertAll(ToDto);
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
