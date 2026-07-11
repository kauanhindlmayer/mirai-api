using Domain.Shared;

namespace Domain.Sprints;

public interface ISprintRepository : IRepository<Sprint>
{
    Task<Sprint?> GetByIdWithWorkItemsAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}