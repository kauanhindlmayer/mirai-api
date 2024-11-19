using Application.Common.Interfaces.Persistence;
using Domain.Tags;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class TagsRepository : Repository<Tag>, ITagsRepository
{
    public TagsRepository(ApplicationDbContext dbContext)
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