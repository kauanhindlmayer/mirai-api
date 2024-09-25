using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.WikiPages;

namespace Mirai.Application.WikiPages.Commands.UpdateWikiPage;

public class UpdateWikiPageCommandHandler(IWikiPagesRepository _wikiPagesRepository)
    : IRequestHandler<UpdateWikiPageCommand, ErrorOr<WikiPage>>
{
    public async Task<ErrorOr<WikiPage>> Handle(
        UpdateWikiPageCommand request,
        CancellationToken cancellationToken)
    {
        var wikiPage = await _wikiPagesRepository.GetByIdAsync(request.WikiPageId, cancellationToken);
        if (wikiPage is null)
        {
            return WikiPageErrors.WikiPageNotFound;
        }

        wikiPage.Update(request.Title, request.Content);
        _wikiPagesRepository.Update(wikiPage);

        return wikiPage;
    }
}