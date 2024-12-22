using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.DeleteComment;

public sealed record DeleteCommentCommand(
    Guid WikiPageId,
    Guid CommentId) : IRequest<ErrorOr<Success>>;