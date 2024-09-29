using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.CreateWikiPage;

public record CreateWikiPageCommand(
    Guid ProjectId,
    string Title,
    string Content,
    Guid? ParentWikiPageId) : IRequest<ErrorOr<WikiPage>>;