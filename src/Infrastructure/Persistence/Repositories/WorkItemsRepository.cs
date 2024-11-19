using Application.Common.Interfaces.Persistence;
using Domain.WorkItems;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class WorkItemsRepository : Repository<WorkItem>, IWorkItemsRepository
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
}