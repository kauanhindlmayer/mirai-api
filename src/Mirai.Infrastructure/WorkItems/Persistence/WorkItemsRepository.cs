using Microsoft.EntityFrameworkCore;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.WorkItems;
using Mirai.Infrastructure.Common.Persistence;

namespace Mirai.Infrastructure.WorkItems.Persistence;

public class WorkItemsRepository(AppDbContext dbContext) : IWorkItemsRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task AddAsync(WorkItem workItem, CancellationToken cancellationToken = default)
    {
        await _dbContext.WorkItems.AddAsync(workItem, cancellationToken);
    }

    public async Task<WorkItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.WorkItems
            .Include(wi => wi.Comments)
            .Include(wi => wi.Tags)
            .FirstOrDefaultAsync(wi => wi.Id == id, cancellationToken);
    }

    // TODO: Refactor this to use a sequence per project.
    public async Task<int> GetNextWorkItemCodeAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var workItemCount = await _dbContext.WorkItems
            .AsNoTracking()
            .Where(wi => wi.ProjectId == projectId)
            .CountAsync(cancellationToken);
        return workItemCount + 1;
    }

    public async Task<List<WorkItem>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.WorkItems.ToListAsync(cancellationToken);
    }

    public void Remove(WorkItem workItem)
    {
        _dbContext.WorkItems.Remove(workItem);
    }

    public void Update(WorkItem workItem)
    {
        _dbContext.WorkItems.Update(workItem);
    }
}