using Domain.Tags;

namespace Application.Common.Interfaces.Persistence;

public interface ITagsRepository : IRepository<Tag>
{
    Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<Tag>> GetByProjectAsync(
        Guid projectId,
        string? searchTerm,
        CancellationToken cancellationToken = default);
    Task<bool> IsTagLinkedToAnyWorkItemsAsync(
        Guid projectId,
        string name,
        CancellationToken cancellationToken = default);
}