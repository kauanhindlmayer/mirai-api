using Application.Abstractions.Authentication;
using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.UpdateComment;

internal sealed class UpdateCommentCommandHandler
    : IRequestHandler<UpdateCommentCommand, ErrorOr<Success>>
{
    private readonly IWikiPageRepository _wikiPageRepository;
    private readonly IUserContext _userContext;

    public UpdateCommentCommandHandler(
        IWikiPageRepository wikiPageRepository,
        IUserContext userContext)
    {
        _wikiPageRepository = wikiPageRepository;
        _userContext = userContext;
    }

    public async Task<ErrorOr<Success>> Handle(
        UpdateCommentCommand command,
        CancellationToken cancellationToken)
    {
        var wikiPage = await _wikiPageRepository.GetByIdWithCommentsAsync(
            command.WikiPageId,
            cancellationToken);

        if (wikiPage is null)
        {
            return WikiPageErrors.NotFound;
        }

        var result = wikiPage.UpdateComment(
            command.CommentId,
            command.Content,
            _userContext.UserId);

        if (result.IsError)
        {
            return result;
        }

        _wikiPageRepository.Update(wikiPage);

        return Result.Success;
    }
}