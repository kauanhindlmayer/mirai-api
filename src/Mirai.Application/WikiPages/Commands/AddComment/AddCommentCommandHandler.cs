using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.WikiPages;

namespace Mirai.Application.WikiPages.Commands.AddComment;

public class AddCommentCommandHandler(
    IWikiPagesRepository _wikiPagesRepository,
    ICurrentUserProvider _currentUserProvider)
    : IRequestHandler<AddCommentCommand, ErrorOr<WikiPageComment>>
{
    public async Task<ErrorOr<WikiPageComment>> Handle(
        AddCommentCommand request,
        CancellationToken cancellationToken)
    {
        var wikiPage = await _wikiPagesRepository.GetByIdAsync(
            request.WikiPageId,
            cancellationToken);

        if (wikiPage is null)
        {
            return Error.NotFound(description: "Work item not found");
        }

        var currentUser = _currentUserProvider.GetCurrentUser();

        var comment = new WikiPageComment(
            wikiPage.Id,
            currentUser.Id,
            request.Content);

        wikiPage.AddComment(comment);
        await _wikiPagesRepository.UpdateAsync(wikiPage, cancellationToken);

        return comment;
    }
}