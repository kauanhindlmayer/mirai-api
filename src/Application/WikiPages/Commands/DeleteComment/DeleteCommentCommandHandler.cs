using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.DeleteComment;

internal sealed class DeleteCommentCommandHandler
    : IRequestHandler<DeleteCommentCommand, ErrorOr<Success>>
{
    private readonly IWikiPageRepository _wikiPageRepository;

    public DeleteCommentCommandHandler(
        IWikiPageRepository wikiPageRepository)
    {
        _wikiPageRepository = wikiPageRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        DeleteCommentCommand command,
        CancellationToken cancellationToken)
    {
        var wikiPage = await _wikiPageRepository.GetByIdWithCommentsAsync(
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

        _wikiPageRepository.Update(wikiPage);

        return Result.Success;
    }
}