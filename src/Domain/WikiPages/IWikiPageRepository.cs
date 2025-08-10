using Domain.Shared;

namespace Domain.WikiPages;

public interface IWikiPageRepository : IRepository<WikiPage>
{
    Task<WikiPage?> GetByIdWithCommentsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<WikiPage?> GetByIdWithSubWikiPagesAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<int> GetViewsForLastDaysAsync(
        Guid wikiPageId,
        int days,
        CancellationToken cancellationToken = default);
}