using Domain.Tags;

namespace Application.Common.Interfaces;

public interface ITagsRepository
{
    Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<Tag>> GetByProjectAsync(Guid projectId, string? searchTerm, CancellationToken cancellationToken = default);
    Task<bool> IsTagLinkedToAnyWorkItemsAsync(Guid projectId, string name, CancellationToken cancellationToken = default);
}