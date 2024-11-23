using Application.Common.Interfaces.Persistence;
using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.DeleteWikiPage;

internal sealed class DeleteWikiPageCommandHandler(IWikiPagesRepository wikiPagesRepository)
    : IRequestHandler<DeleteWikiPageCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        DeleteWikiPageCommand command,
        CancellationToken cancellationToken)
    {
        var wikiPage = await wikiPagesRepository.GetByIdAsync(
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

        wikiPagesRepository.Remove(wikiPage);

        return Result.Success;
    }
}