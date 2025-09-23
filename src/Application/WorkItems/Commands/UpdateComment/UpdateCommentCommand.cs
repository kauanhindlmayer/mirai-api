using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.UpdateComment;

public sealed record UpdateCommentCommand(
    Guid WorkItemId,
    Guid CommentId,
    string Content) : IRequest<ErrorOr<Success>>;