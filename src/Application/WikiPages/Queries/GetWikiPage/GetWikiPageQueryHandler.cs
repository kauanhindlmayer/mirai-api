using Application.Common.Interfaces;
using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Queries.GetWikiPage;

public class GetWikiPageQueryHandler(IWikiPagesRepository _wikiPagesRepository)
    : IRequestHandler<GetWikiPageQuery, ErrorOr<WikiPage>>
{
    public async Task<ErrorOr<WikiPage>> Handle(
        GetWikiPageQuery query,
        CancellationToken cancellationToken)
    {
        var wikiPage = await _wikiPagesRepository.GetByIdAsync(
            query.WikiPageId,
            cancellationToken);

        if (wikiPage is null)
        {
            return WikiPageErrors.WikiPageNotFound;
        }

        return wikiPage;
    }
}