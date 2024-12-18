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

    Task<PagedList<WorkItem>> PaginatedListAsync(
        Guid projectId,
        int pageNumber,
        int pageSize,
        string? sortColumn,
        string? sortOrder,
        string? searchTerm,
        CancellationToken cancellationToken = default);
}