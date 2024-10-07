using Application.Common.Interfaces;
using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.DeleteWikiPage;

public class DeleteWikiPageCommandHandler(IWikiPagesRepository _wikiPagesRepository)
    : IRequestHandler<DeleteWikiPageCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        DeleteWikiPageCommand command,
        CancellationToken cancellationToken)
    {
        var wikiPage = await _wikiPagesRepository.GetByIdAsync(
            command.WikiPageId,
            cancellationToken);

        if (wikiPage is null)
        {
            return WikiPageErrors.NotFound;
        }

        if (wikiPage.SubWikiPages.Count > 0)
        {
            return WikiPageErrors.HasSubWikiPages;
        }

        _wikiPagesRepository.Remove(wikiPage);

        return Result.Success;
    }
}