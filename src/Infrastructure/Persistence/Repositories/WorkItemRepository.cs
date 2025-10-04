using Domain.WorkItems;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class WorkItemRepository : Repository<WorkItem>, IWorkItemRepository
{
    public WorkItemRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<WorkItem?> GetByIdWithCommentsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.WorkItems
            .Include(wi => wi.Comments)
            .FirstOrDefaultAsync(wi => wi.Id == id, cancellationToken);
    }

    public async Task<WorkItem?> GetByIdWithTagsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.WorkItems
            .Include(wi => wi.Tags)
            .FirstOrDefaultAsync(wi => wi.Id == id, cancellationToken);
    }

    public async Task<WorkItem?> GetByIdWithLinksAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.WorkItems
            .Include(wi => wi.OutgoingLinks)
            .FirstOrDefaultAsync(wi => wi.Id == id, cancellationToken);
    }

    public async Task<int> GetNextWorkItemCodeAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var workItemCount = await _dbContext.WorkItems
            .AsNoTracking()
            .Where(wi => wi.ProjectId == projectId)
            .CountAsync(cancellationToken);

        return workItemCount + 1;
    }

    public Task<List<WorkItem>> ListByTagIdAsync(
        Guid tagId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.WorkItems
            .AsNoTracking()
            .Where(wi => wi.Tags.Any(t => t.Id == tagId))
            .ToListAsync(cancellationToken);
    }
}
