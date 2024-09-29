using Domain.WorkItems;

namespace Application.Common.Interfaces;

public interface IWorkItemsRepository
{
    Task AddAsync(WorkItem workItem, CancellationToken cancellationToken = default);
    Task<WorkItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<WorkItem>> ListAsync(CancellationToken cancellationToken = default);
    void Remove(WorkItem workItem);
    void Update(WorkItem workItem);
    Task<int> GetNextWorkItemCodeAsync(Guid projectId, CancellationToken cancellationToken = default);
}