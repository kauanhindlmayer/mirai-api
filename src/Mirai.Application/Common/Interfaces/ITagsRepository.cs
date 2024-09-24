using Mirai.Domain.Tags;

namespace Mirai.Application.Common.Interfaces;

public interface ITagsRepository
{
    Task AddAsync(Tag tag, CancellationToken cancellationToken);
    Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task RemoveAsync(Tag tag, CancellationToken cancellationToken);
}