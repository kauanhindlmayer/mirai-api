using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.DeleteWikiPage;

internal sealed class DeleteWikiPageCommandHandler
    : IRequestHandler<DeleteWikiPageCommand, ErrorOr<Success>>
{
    private readonly IWikiPagesRepository _wikiPagesRepository;

    public DeleteWikiPageCommandHandler(
        IWikiPagesRepository wikiPagesRepository)
    {
        _wikiPagesRepository = wikiPagesRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        DeleteWikiPageCommand command,
        CancellationToken cancellationToken)
    {
        var wikiPage = await _wikiPagesRepository.GetByIdWithSubWikiPagesAsync(
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