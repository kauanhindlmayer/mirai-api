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
        return await _dbContext.WorkItems.FindAsync(workItemId, cancellationToken);
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