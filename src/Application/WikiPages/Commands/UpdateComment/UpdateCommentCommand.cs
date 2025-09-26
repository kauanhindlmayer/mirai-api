using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.UpdateComment;

public sealed record UpdateCommentCommand(
    Guid WikiPageId,
    Guid CommentId,
    string Content) : IRequest<ErrorOr<Success>>;