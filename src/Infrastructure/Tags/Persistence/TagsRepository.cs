using Application.Common.Interfaces;
using Domain.Tags;
using Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tags.Persistence;

public class TagsRepository : Repository<Tag>, ITagsRepository
{
    public TagsRepository(AppDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<Tag?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Tags.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
    }

    public Task<List<Tag>> GetByProjectAsync(
        Guid projectId,
        string? searchTerm,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Tags
            .AsNoTracking()
            .Where(t => t.ProjectId == projectId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(t => t.Name.Contains(searchTerm));
        }

        return query.ToListAsync(cancellationToken);
    }

    public async Task<bool> IsTagLinkedToAnyWorkItemsAsync(
        Guid projectId,
        string name,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tags
            .AsNoTracking()
            .Include(t => t.WorkItems)
            .AnyAsync(t => t.ProjectId == projectId && t.Name == name && t.WorkItems.Count != 0, cancellationToken);
    }
}