using Mirai.Domain.Tags;

namespace Mirai.Application.Common.Interfaces;

public interface ITagsRepository
{
    Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> HasWorkItemsAssociatedAsync(Guid projectId, string name, CancellationToken cancellationToken = default);
}