using Mirai.Domain.WorkItems;

namespace Mirai.Application.Common.Interfaces;

public interface ITagsRepository
{
    Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken);
}