using Application.Common.Interfaces.Persistence;
using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.DeleteComment;

internal sealed class DeleteCommentCommandHandler(
    IWikiPagesRepository wikiPagesRepository)
    : IRequestHandler<DeleteCommentCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        DeleteCommentCommand command,
        CancellationToken cancellationToken)
    {
        var wikiPage = await wikiPagesRepository.GetByIdAsync(
            command.WikiPageId,
            cancellationToken);

        if (wikiPage is null)
        {
            return WikiPageErrors.NotFound;
        }

        var result = wikiPage.RemoveComment(command.CommentId);
        if (result.IsError)
        {
            return result.Errors;
        }

        wikiPagesRepository.Update(wikiPage);

        return Result.Success;
    }
}