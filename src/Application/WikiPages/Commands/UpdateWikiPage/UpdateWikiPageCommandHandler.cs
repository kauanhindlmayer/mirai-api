using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.UpdateWikiPage;

internal sealed class UpdateWikiPageCommandHandler
    : IRequestHandler<UpdateWikiPageCommand, ErrorOr<Guid>>
{
    private readonly IWikiPageRepository _wikiPageRepository;

    public UpdateWikiPageCommandHandler(
        IWikiPageRepository wikiPageRepository)
    {
        _wikiPageRepository = wikiPageRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(
        UpdateWikiPageCommand command,
        CancellationToken cancellationToken)
    {
        var wikiPage = await _wikiPageRepository.GetByIdAsync(
            command.WikiPageId,
            cancellationToken);

        if (wikiPage is null)
        {
            return WikiPageErrors.NotFound;
        }

        wikiPage.Update(command.Title, command.Content);
        _wikiPageRepository.Update(wikiPage);

        return wikiPage.Id;
    }
}