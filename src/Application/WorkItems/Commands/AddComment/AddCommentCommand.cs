using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.AddComment;

public sealed record AddCommentCommand(
    Guid WorkItemId,
    string Content) : IRequest<ErrorOr<WorkItemComment>>;