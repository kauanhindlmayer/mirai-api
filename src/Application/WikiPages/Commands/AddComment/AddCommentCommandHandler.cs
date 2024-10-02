using Application.Common.Interfaces;
using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.AddComment;

public class AddCommentCommandHandler(
    IWikiPagesRepository _wikiPagesRepository,
    ICurrentUserProvider _currentUserProvider)
    : IRequestHandler<AddCommentCommand, ErrorOr<WikiPageComment>>
{
    public async Task<ErrorOr<WikiPageComment>> Handle(
        AddCommentCommand command,
        CancellationToken cancellationToken)
    {
        var wikiPage = await _wikiPagesRepository.GetByIdAsync(
            command.WikiPageId,
            cancellationToken);

        if (wikiPage is null)
        {
            return WikiPageErrors.WikiPageNotFound;
        }

        var currentUser = _currentUserProvider.GetCurrentUser();

        var comment = new WikiPageComment(
            wikiPage.Id,
            currentUser.Id,
            command.Content);

        wikiPage.AddComment(comment);
        _wikiPagesRepository.Update(wikiPage);

        return comment;
    }
}