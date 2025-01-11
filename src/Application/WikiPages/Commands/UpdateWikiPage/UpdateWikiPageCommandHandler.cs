using Application.Common.Interfaces.Persistence;
using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.UpdateWikiPage;

internal sealed class UpdateWikiPageCommandHandler : IRequestHandler<UpdateWikiPageCommand, ErrorOr<Guid>>
{
    private readonly IWikiPagesRepository _wikiPagesRepository;

    public UpdateWikiPageCommandHandler(IWikiPagesRepository wikiPagesRepository)
    {
        _wikiPagesRepository = wikiPagesRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(
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

        return wikiPage.Id;
    }
}