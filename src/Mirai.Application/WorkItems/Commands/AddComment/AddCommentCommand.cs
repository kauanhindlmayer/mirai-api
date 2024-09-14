using ErrorOr;
using MediatR;
using Mirai.Domain.WorkItems;

namespace Mirai.Application.WorkItems.Commands.AddComment;

public record AddCommentCommand(Guid WorkItemId, string Content)
    : IRequest<ErrorOr<WorkItemComment>>;