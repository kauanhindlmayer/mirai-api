using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.CreateWikiPage;

public sealed record CreateWikiPageCommand(
    Guid ProjectId,
    string Title,
    string Content,
    Guid? ParentWikiPageId) : IRequest<ErrorOr<Guid>>;