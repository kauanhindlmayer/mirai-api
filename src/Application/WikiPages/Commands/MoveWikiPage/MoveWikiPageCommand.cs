using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.MoveWikiPage;

public record MoveWikiPageCommand(
    Guid ProjectId,
    Guid WikiPageId,
    Guid? TargetParentId,
    int TargetPosition) : IRequest<ErrorOr<Success>>;