using Application.Common.Interfaces;
using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.DeleteWikiPage;

public class DeleteWikiPageCommandHandler(IWikiPagesRepository _wikiPagesRepository)
    : IRequestHandler<DeleteWikiPageCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        DeleteWikiPageCommand request,
        CancellationToken cancellationToken)
    {
        var wikiPage = await _wikiPagesRepository.GetByIdAsync(
            request.WikiPageId,
            cancellationToken);

        if (wikiPage is null)
        {
            return WikiPageErrors.WikiPageNotFound;
        }

        if (wikiPage.SubWikiPages.Count > 0)
        {
            return WikiPageErrors.WikiPageHasSubWikiPages;
        }

        _wikiPagesRepository.Remove(wikiPage);
        return Result.Success;
    }
}