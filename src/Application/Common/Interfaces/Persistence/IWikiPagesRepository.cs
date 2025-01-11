using Domain.WikiPages;

namespace Application.Common.Interfaces.Persistence;

public interface IWikiPagesRepository : IRepository<WikiPage>
{
    Task<WikiPage?> GetByIdWithCommentsAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    Task LogViewAsync(
        Guid wikiPageId,
        Guid viewerId,
        CancellationToken cancellationToken = default);
    Task<int> GetViewsForLastDaysAsync(
        Guid wikiPageId,
        int days,
        CancellationToken cancellationToken = default);
}