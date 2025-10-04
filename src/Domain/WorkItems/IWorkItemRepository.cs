using Domain.Shared;

namespace Domain.WorkItems;

public interface IWorkItemRepository : IRepository<WorkItem>
{
    Task<WorkItem?> GetByIdWithCommentsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<WorkItem?> GetByIdWithTagsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<WorkItem?> GetByIdWithLinksAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<int> GetNextWorkItemCodeAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);

    Task<List<WorkItem>> ListByTagIdAsync(
        Guid tagId,
        CancellationToken cancellationToken = default);
}