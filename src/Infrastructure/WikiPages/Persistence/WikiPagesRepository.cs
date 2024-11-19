using Application.Common.Interfaces;
using Domain.WikiPages;
using Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.WikiPages.Persistence;

public class WikiPagesRepository : Repository<WikiPage>, IWikiPagesRepository
{
    public WikiPagesRepository(AppDbContext dbContext)
        : base(dbContext)
    {
    }

    public new async Task<WikiPage?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.WikiPages
            .Include(wp => wp.SubWikiPages)
            .Include(wp => wp.Comments)
            .FirstOrDefaultAsync(wp => wp.Id == id, cancellationToken);
    }
}