using Application.Common.Interfaces;
using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.AddComment;

public class AddCommentCommandHandler(
    IWikiPagesRepository _wikiPagesRepository,
    IUserContext _userContext)
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
            return WikiPageErrors.NotFound;
        }

        var comment = new WikiPageComment(
            wikiPage.Id,
            _userContext.UserId,
            command.Content);

        wikiPage.AddComment(comment);
        _wikiPagesRepository.Update(wikiPage);

        return comment;
    }
}