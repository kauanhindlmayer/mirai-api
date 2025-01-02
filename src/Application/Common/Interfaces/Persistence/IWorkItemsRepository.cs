using Domain.WorkItems;

namespace Application.Common.Interfaces.Persistence;

public interface IWorkItemsRepository : IRepository<WorkItem>
{
    Task<List<WorkItem>> ListAsync(CancellationToken cancellationToken = default);
    Task<int> GetNextWorkItemCodeAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);
    Task<List<WorkItem>> SearchAsync(
        Guid projectId,
        float[] searchTermEmbedding,
        int topK = 10,
        CancellationToken cancellationToken = default);

    Task<PaginatedList<WorkItem>> PaginatedListAsync(
        Guid projectId,
        int pageNumber,
        int pageSize,
        string? sortField,
        string? sortOrder,
        string? searchTerm,
        CancellationToken cancellationToken = default);
}