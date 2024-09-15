using Mirai.Domain.WikiPages;

namespace Mirai.Application.Common.Interfaces;

public interface IWikiPagesRepository
{
    Task AddAsync(WikiPage wikiPage, CancellationToken cancellationToken);
    Task<WikiPage?> GetByIdAsync(Guid wikiPageId, CancellationToken cancellationToken);
    Task<List<WikiPage>> ListAsync(CancellationToken cancellationToken);
    Task RemoveAsync(WikiPage wikiPage, CancellationToken cancellationToken);
    Task UpdateAsync(WikiPage wikiPage, CancellationToken cancellationToken);
}