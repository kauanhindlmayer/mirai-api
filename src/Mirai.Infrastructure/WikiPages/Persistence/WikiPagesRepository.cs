using Microsoft.EntityFrameworkCore;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.WikiPages;
using Mirai.Infrastructure.Common.Persistence;

namespace Mirai.Infrastructure.WikiPages.Persistence;

public class WikiPagesRepository(AppDbContext dbContext) : IWikiPagesRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task AddAsync(WikiPage wikiPage, CancellationToken cancellationToken = default)
    {
        await _dbContext.WikiPages.AddAsync(wikiPage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<WikiPage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.WikiPages
            .Include(wp => wp.SubWikiPages)
            .Include(wp => wp.Comments)
            .FirstOrDefaultAsync(wp => wp.Id == id, cancellationToken);
    }

    public async Task<List<WikiPage>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.WikiPages.ToListAsync(cancellationToken);
    }

    public async Task RemoveAsync(WikiPage wikiPage, CancellationToken cancellationToken = default)
    {
        _dbContext.WikiPages.Remove(wikiPage);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(WikiPage wikiPage, CancellationToken cancellationToken = default)
    {
        _dbContext.WikiPages.Update(wikiPage);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}