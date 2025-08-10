using Domain.WikiPages;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class WikiPageRepository : Repository<WikiPage>, IWikiPageRepository
{
    public WikiPageRepository(ApplicationDbContext dbContext)
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

    public async Task<WikiPage?> GetByIdWithSubWikiPagesAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.WikiPages
            .Include(wp => wp.SubWikiPages)
            .FirstOrDefaultAsync(wp => wp.Id == id, cancellationToken);
    }

    public Task<int> GetViewsForLastDaysAsync(
        Guid wikiPageId,
        int days,
        CancellationToken cancellationToken = default)
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-days);
        return _dbContext.WikiPageViews
            .AsNoTracking()
            .CountAsync(
                wpv => wpv.WikiPageId == wikiPageId &&
                       wpv.ViewedAt >= startDate,
                cancellationToken);
    }
}