using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.DeleteWikiPage;

internal sealed class DeleteWikiPageCommandHandler
    : IRequestHandler<DeleteWikiPageCommand, ErrorOr<Success>>
{
    private readonly IWikiPageRepository _wikiPageRepository;

    public DeleteWikiPageCommandHandler(
        IWikiPageRepository wikiPageRepository)
    {
        _wikiPageRepository = wikiPageRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        DeleteWikiPageCommand command,
        CancellationToken cancellationToken)
    {
        var wikiPage = await _wikiPageRepository.GetByIdWithSubWikiPagesAsync(
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

        _wikiPageRepository.Remove(wikiPage);

        return Result.Success;
    }
}