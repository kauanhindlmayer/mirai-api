using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.AddComment;

internal sealed class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, ErrorOr<Guid>>
{
    private readonly IWikiPagesRepository _wikiPagesRepository;
    private readonly IUserContext _userContext;

    public AddCommentCommandHandler(
        IWikiPagesRepository wikiPagesRepository,
        IUserContext userContext)
    {
        _wikiPagesRepository = wikiPagesRepository;
        _userContext = userContext;
    }

    public async Task<ErrorOr<Guid>> Handle(
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

        return comment.Id;
    }
}