using Application.Common.Interfaces.Persistence;
using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.UpdateWikiPage;

internal sealed class UpdateWikiPageCommandHandler(IWikiPagesRepository wikiPagesRepository)
    : IRequestHandler<UpdateWikiPageCommand, ErrorOr<WikiPage>>
{
    public async Task<ErrorOr<WikiPage>> Handle(
        UpdateWikiPageCommand command,
        CancellationToken cancellationToken)
    {
        var wikiPage = await wikiPagesRepository.GetByIdAsync(
            command.WikiPageId,
            cancellationToken);

        if (wikiPage is null)
        {
            return WikiPageErrors.NotFound;
        }

        wikiPage.Update(command.Title, command.Content);
        wikiPagesRepository.Update(wikiPage);

        return wikiPage;
    }
}