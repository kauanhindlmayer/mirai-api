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
        AddCommentCommand request,
        CancellationToken cancellationToken)
    {
        var wikiPage = await _wikiPagesRepository.GetByIdAsync(
            request.WikiPageId,
            cancellationToken);

        if (wikiPage is null)
        {
            return WikiPageErrors.WikiPageNotFound;
        }

        var currentUser = _currentUserProvider.GetCurrentUser();

        var comment = new WikiPageComment(
            wikiPage.Id,
            currentUser.Id,
            request.Content);

        wikiPage.AddComment(comment);
        _wikiPagesRepository.Update(wikiPage);

        return comment;
    }
}