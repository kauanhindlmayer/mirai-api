using Application.Common.Interfaces.Persistence;
using Domain.WorkItems;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class WorkItemsRepository : Repository<WorkItem>, IWorkItemsRepository
{
    public WorkItemsRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public new async Task<WorkItem?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.WorkItems
            .Include(wi => wi.Comments)
            .Include(wi => wi.Tags)
            .FirstOrDefaultAsync(wi => wi.Id == id, cancellationToken);
    }

    // TODO: Refactor this to use a sequence per project.
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

    public Task<List<WorkItem>> SearchAsync(
        Guid projectId,
        float[] searchTermEmbedding,
        int topK = 10,
        CancellationToken cancellationToken = default)
    {
        var searchTermVector = new Vector(searchTermEmbedding);

        return _dbContext.WorkItems
            .AsNoTracking()
            .Where(wi => wi.ProjectId == projectId && wi.SearchVector != null)
            .OrderBy(wi => wi.SearchVector!.CosineDistance(searchTermVector))
            .Take(topK)
            .ToListAsync(cancellationToken);
    }
}
