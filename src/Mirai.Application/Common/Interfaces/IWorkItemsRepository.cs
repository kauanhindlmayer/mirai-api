using Mirai.Domain.WorkItems;

namespace Mirai.Application.Common.Interfaces;

public interface IWorkItemsRepository
{
    Task AddAsync(WorkItem workItem, CancellationToken cancellationToken = default);
    Task<WorkItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<WorkItem>> ListAsync(CancellationToken cancellationToken = default);
    Task RemoveAsync(WorkItem workItem, CancellationToken cancellationToken = default);
    Task UpdateAsync(WorkItem workItem, CancellationToken cancellationToken = default);
    Task<int> GetNextWorkItemCodeAsync(Guid projectId, CancellationToken cancellationToken = default);
}