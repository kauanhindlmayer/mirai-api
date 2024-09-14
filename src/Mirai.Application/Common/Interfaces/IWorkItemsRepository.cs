using Mirai.Domain.WorkItems;

namespace Mirai.Application.Common.Interfaces;

public interface IWorkItemsRepository
{
    Task AddAsync(WorkItem workItem, CancellationToken cancellationToken);
    Task<WorkItem?> GetByIdAsync(Guid workItemId, CancellationToken cancellationToken);
    Task<List<WorkItem>> ListAsync(CancellationToken cancellationToken);
    Task RemoveAsync(WorkItem workItem, CancellationToken cancellationToken);
    Task UpdateAsync(WorkItem workItem, CancellationToken cancellationToken);
}