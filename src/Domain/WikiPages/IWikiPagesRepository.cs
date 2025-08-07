using Domain.Common;

namespace Domain.WikiPages;

public interface IWikiPagesRepository : IRepository<WikiPage>
{
    Task<WikiPage?> GetByIdWithCommentsAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    Task<WikiPage?> GetByIdWithSubWikiPagesAsync(
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