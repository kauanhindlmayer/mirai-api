using Microsoft.EntityFrameworkCore;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.WorkItems;
using Mirai.Infrastructure.Common.Persistence;

namespace Mirai.Infrastructure.Tags.Persistence;

public class TagsRepository(AppDbContext dbContext) : ITagsRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task AddAsync(Tag tag, CancellationToken cancellationToken)
    {
        await _dbContext.Tags.AddAsync(tag, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return _dbContext.Tags.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
    }

    public async Task RemoveAsync(Tag tag, CancellationToken cancellationToken)
    {
        _dbContext.Tags.Remove(tag);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}