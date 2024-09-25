using Microsoft.EntityFrameworkCore;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Tags;
using Mirai.Infrastructure.Common.Persistence;

namespace Mirai.Infrastructure.Tags.Persistence;

public class TagsRepository(AppDbContext dbContext) : ITagsRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return _dbContext.Tags.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
    }

    public async Task<bool> HasWorkItemsAssociatedAsync(Guid projectId, string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tags
            .AsNoTracking()
            .Include(t => t.WorkItems)
            .AnyAsync(t => t.ProjectId == projectId && t.Name == name && t.WorkItems.Count != 0, cancellationToken);
    }
}