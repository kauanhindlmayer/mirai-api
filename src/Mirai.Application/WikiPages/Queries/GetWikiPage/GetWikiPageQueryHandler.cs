using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.WikiPages;

namespace Mirai.Application.WikiPages.Queries.GetWikiPage;

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
            return Error.NotFound(description: "WikiPage not found");
        }

        return wikiPage;
    }
}