using Mirai.Domain.WikiPages;

namespace Mirai.Application.Common.Interfaces;

public interface IWikiPagesRepository
{
    Task AddAsync(WikiPage wikiPage, CancellationToken cancellationToken = default);
    Task<WikiPage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<WikiPage>> ListAsync(CancellationToken cancellationToken = default);
    Task RemoveAsync(WikiPage wikiPage, CancellationToken cancellationToken = default);
    Task UpdateAsync(WikiPage wikiPage, CancellationToken cancellationToken = default);
}