using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.UpdateWikiPage;

public sealed record UpdateWikiPageCommand(
    Guid WikiPageId,
    string Title,
    string Content) : IRequest<ErrorOr<WikiPage>>;