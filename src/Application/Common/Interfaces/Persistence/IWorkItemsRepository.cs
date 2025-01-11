using Domain.WorkItems;

namespace Application.Common.Interfaces.Persistence;

public interface IWorkItemsRepository : IRepository<WorkItem>
{
    Task<int> GetNextWorkItemCodeAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);
    Task<List<WorkItem>> SearchAsync(
        Guid projectId,
        float[] searchTermEmbedding,
        int topK = 10,
        CancellationToken cancellationToken = default);
}