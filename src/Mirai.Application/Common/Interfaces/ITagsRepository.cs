using Mirai.Domain.Tags;

namespace Mirai.Application.Common.Interfaces;

public interface ITagsRepository
{
    Task AddAsync(Tag tag, CancellationToken cancellationToken = default);
    Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task RemoveAsync(Tag tag, CancellationToken cancellationToken = default);
}