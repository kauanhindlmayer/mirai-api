using Mirai.Domain.WorkItems;

namespace Mirai.Application.Common.Interfaces;

public interface ITagsRepository
{
    Task AddAsync(Tag tag, CancellationToken cancellationToken);
    Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken);
}