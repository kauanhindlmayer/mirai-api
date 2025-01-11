using Domain.WorkItems;

namespace Application.Common.Interfaces.Persistence;

public interface IWorkItemsRepository : IRepository<WorkItem>
{
    Task<WorkItem?> GetByIdWithCommentsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WorkItem?> GetByIdWithTagsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> GetNextWorkItemCodeAsync(Guid projectId, CancellationToken cancellationToken = default);
}