using Application.Common.Interfaces.Persistence;
using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.UpdateWikiPage;

internal sealed class UpdateWikiPageCommandHandler(IWikiPagesRepository _wikiPagesRepository)
    : IRequestHandler<UpdateWikiPageCommand, ErrorOr<WikiPage>>
{
    public async Task<ErrorOr<WikiPage>> Handle(
        UpdateWikiPageCommand command,
        CancellationToken cancellationToken)
    {
        var wikiPage = await _wikiPagesRepository.GetByIdAsync(
            command.WikiPageId,
            cancellationToken);

        if (wikiPage is null)
        {
            return WikiPageErrors.NotFound;
        }

        wikiPage.Update(command.Title, command.Content);
        _wikiPagesRepository.Update(wikiPage);

        return wikiPage;
    }
}