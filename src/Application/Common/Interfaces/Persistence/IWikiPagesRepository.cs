using Domain.WikiPages;

namespace Application.Common.Interfaces.Persistence;

public interface IWikiPagesRepository : IRepository<WikiPage>
{
    Task<List<WikiPage>> ListAsync(CancellationToken cancellationToken = default);
}