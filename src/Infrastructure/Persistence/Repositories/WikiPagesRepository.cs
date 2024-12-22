using Application.Common.Interfaces.Persistence;
using Domain.WikiPages;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class WikiPagesRepository : Repository<WikiPage>, IWikiPagesRepository
{
    public WikiPagesRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<WikiPage?> GetByIdWithCommentsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.WikiPages
            .Include(wp => wp.Author)
            .Include(wp => wp.SubWikiPages)
            .Include(wp => wp.Comments)
                .ThenInclude(c => c.Author)
            .FirstOrDefaultAsync(wp => wp.Id == id, cancellationToken);
    }

    public Task LogViewAsync(
        Guid wikiPageId,
        Guid viewerId,
        CancellationToken cancellationToken = default)
    {
        var wikiPageView = new WikiPageView(wikiPageId, viewerId);
        _dbContext.WikiPageViews.Add(wikiPageView);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<int> GetViewsForLastDaysAsync(
        Guid wikiPageId,
        int days,
        CancellationToken cancellationToken = default)
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-days);
        return _dbContext.WikiPageViews
            .AsNoTracking()
            .CountAsync(wpv => wpv.WikiPageId == wikiPageId && wpv.ViewedAt >= startDate, cancellationToken);
    }
}