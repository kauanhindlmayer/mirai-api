using Domain.WorkItems;

namespace Application.Common.Interfaces.Persistence;

public interface IWorkItemsRepository
{
    Task AddAsync(WorkItem workItem, CancellationToken cancellationToken = default);
    Task<WorkItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<WorkItem>> ListAsync(CancellationToken cancellationToken = default);
    void Remove(WorkItem workItem);
    void Update(WorkItem workItem);
    Task<int> GetNextWorkItemCodeAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);
    Task<List<WorkItemSummary>> SearchAsync(
        Guid projectId,
        float[] searchTermEmbedding,
        int topK = 10,
        CancellationToken cancellationToken = default);
}