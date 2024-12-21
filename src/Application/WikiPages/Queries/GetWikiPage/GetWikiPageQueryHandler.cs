using Application.Common.Interfaces.Persistence;
using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Queries.GetWikiPage;

internal sealed class GetWikiPageQueryHandler(IWikiPagesRepository wikiPagesRepository)
    : IRequestHandler<GetWikiPageQuery, ErrorOr<WikiPage>>
{
    public async Task<ErrorOr<WikiPage>> Handle(
        GetWikiPageQuery query,
        CancellationToken cancellationToken)
    {
        var wikiPage = await wikiPagesRepository.GetByIdWithCommentsAsync(
            query.WikiPageId,
            cancellationToken);

        if (wikiPage is null)
        {
            return WikiPageErrors.NotFound;
        }

        return wikiPage;
    }
}