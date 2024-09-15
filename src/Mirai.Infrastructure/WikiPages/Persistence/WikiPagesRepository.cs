using Microsoft.EntityFrameworkCore;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.WikiPages;
using Mirai.Infrastructure.Common.Persistence;

namespace Mirai.Infrastructure.WikiPages.Persistence;

public class WikiPagesRepository(AppDbContext dbContext) : IWikiPagesRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task AddAsync(WikiPage wikiPage, CancellationToken cancellationToken)
    {
        await _dbContext.WikiPages.AddAsync(wikiPage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<WikiPage?> GetByIdAsync(Guid wikiPageId, CancellationToken cancellationToken)
    {
        return await _dbContext.WikiPages
            .Include(wp => wp.Comments)
            .FirstOrDefaultAsync(wp => wp.Id == wikiPageId, cancellationToken);
    }

    public async Task<List<WikiPage>> ListAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.WikiPages.ToListAsync(cancellationToken);
    }

    public async Task RemoveAsync(WikiPage wikiPage, CancellationToken cancellationToken)
    {
        _dbContext.WikiPages.Remove(wikiPage);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(WikiPage wikiPage, CancellationToken cancellationToken)
    {
        _dbContext.WikiPages.Update(wikiPage);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}