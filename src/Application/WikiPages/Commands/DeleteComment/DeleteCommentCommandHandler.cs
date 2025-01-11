using Application.Common.Interfaces.Persistence;
using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.DeleteComment;

internal sealed class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, ErrorOr<Success>>
{
    private readonly IWikiPagesRepository _wikiPagesRepository;

    public DeleteCommentCommandHandler(IWikiPagesRepository wikiPagesRepository)
    {
        _wikiPagesRepository = wikiPagesRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        DeleteCommentCommand command,
        CancellationToken cancellationToken)
    {
        var wikiPage = await _wikiPagesRepository.GetByIdWithCommentsAsync(
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

        _wikiPagesRepository.Update(wikiPage);

        return Result.Success;
    }
}