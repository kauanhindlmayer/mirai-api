using Domain.Tags;

namespace Application.Common.Interfaces.Persistence;

public interface ITagsRepository : IRepository<Tag>
{
    Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<Tag>> ListByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
    void RemoveRange(IEnumerable<Tag> tags, CancellationToken cancellationToken = default);
}