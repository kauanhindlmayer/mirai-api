using Application.Abstractions.Authentication;
using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.AddComment;

internal sealed class AddCommentCommandHandler
    : IRequestHandler<AddCommentCommand, ErrorOr<Guid>>
{
    private readonly IWikiPageRepository _wikiPageRepository;
    private readonly IUserContext _userContext;

    public AddCommentCommandHandler(
        IWikiPageRepository wikiPageRepository,
        IUserContext userContext)
    {
        _wikiPageRepository = wikiPageRepository;
        _userContext = userContext;
    }

    public async Task<ErrorOr<Guid>> Handle(
        AddCommentCommand command,
        CancellationToken cancellationToken)
    {
        var wikiPage = await _wikiPageRepository.GetByIdAsync(
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
        _wikiPageRepository.Update(wikiPage);

        return comment.Id;
    }
}