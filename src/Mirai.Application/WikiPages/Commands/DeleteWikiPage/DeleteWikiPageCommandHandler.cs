using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;

namespace Mirai.Application.WikiPages.Commands.DeleteWikiPage;

public class DeleteWikiPageCommandHandler(IWikiPagesRepository _wikiPagesRepository)
    : IRequestHandler<DeleteWikiPageCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        DeleteWikiPageCommand request,
        CancellationToken cancellationToken)
    {
        var wikiPage = await _wikiPagesRepository.GetByIdAsync(request.WikiPageId, cancellationToken);
        if (wikiPage is null)
        {
            return Error.NotFound(description: $"WikiPage not found");
        }

        if (wikiPage.SubWikiPages.Count > 0)
        {
            return Error.Validation(description: $"WikiPage has sub wiki pages");
        }

        await _wikiPagesRepository.RemoveAsync(wikiPage, cancellationToken);
        return Result.Success;
    }
}