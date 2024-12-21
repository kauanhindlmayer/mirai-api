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
            .Include(wp => wp.SubWikiPages)
            .Include(wp => wp.Comments)
                .ThenInclude(c => c.Author)
            .FirstOrDefaultAsync(wp => wp.Id == id, cancellationToken);
    }
}