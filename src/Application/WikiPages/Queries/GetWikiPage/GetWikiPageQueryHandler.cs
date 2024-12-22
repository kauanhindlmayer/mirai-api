using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Queries.GetWikiPage;

internal sealed class GetWikiPageQueryHandler(
    IWikiPagesRepository wikiPagesRepository,
    IUserContext userContext)
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

        await wikiPagesRepository.LogViewAsync(
            wikiPage.Id,
            userContext.UserId,
            cancellationToken);

        return wikiPage;
    }
}