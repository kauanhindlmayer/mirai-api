using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.AddComment;

internal sealed class AddCommentCommandHandler(
    IWikiPagesRepository wikiPagesRepository,
    IUserContext userContext)
    : IRequestHandler<AddCommentCommand, ErrorOr<WikiPageComment>>
{
    public async Task<ErrorOr<WikiPageComment>> Handle(
        AddCommentCommand command,
        CancellationToken cancellationToken)
    {
        var wikiPage = await wikiPagesRepository.GetByIdAsync(
            command.WikiPageId,
            cancellationToken);

        if (wikiPage is null)
        {
            return WikiPageErrors.NotFound;
        }

        var comment = new WikiPageComment(
            wikiPage.Id,
            userContext.UserId,
            command.Content);

        wikiPage.AddComment(comment);
        wikiPagesRepository.Update(wikiPage);

        return comment;
    }
}