using Microsoft.EntityFrameworkCore;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.WorkItems;
using Mirai.Infrastructure.Common.Persistence;

namespace Mirai.Infrastructure.WorkItems.Persistence;

public class WorkItemsRepository(AppDbContext dbContext) : IWorkItemsRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task AddAsync(WorkItem workItem, CancellationToken cancellationToken)
    {
        await _dbContext.WorkItems.AddAsync(workItem, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<WorkItem?> GetByIdAsync(Guid workItemId, CancellationToken cancellationToken)
    {
        return await _dbContext.WorkItems
            .Include(wi => wi.Comments)
            .Include(wi => wi.Tags)
            .FirstOrDefaultAsync(wi => wi.Id == workItemId, cancellationToken);
    }

    public record NextValueResult
    {
        public long Value { get; set; }
    }

    // TODO: Refactor this to use a sequence per project.
    public async Task<int> GetNextWorkItemCodeAsync(Guid projectId, CancellationToken cancellationToken)
    {
        var workItemCount = await _dbContext.WorkItems
            .AsNoTracking()
            .Where(wi => wi.ProjectId == projectId)
            .CountAsync(cancellationToken);
        return workItemCount + 1;
    }

    public async Task<List<WorkItem>> ListAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.WorkItems.ToListAsync(cancellationToken);
    }

    public async Task RemoveAsync(WorkItem workItem, CancellationToken cancellationToken)
    {
        _dbContext.WorkItems.Remove(workItem);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(WorkItem workItem, CancellationToken cancellationToken)
    {
        _dbContext.WorkItems.Update(workItem);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}