using ErrorOr;
using MediatR;

namespace Mirai.Application.WorkItems.Commands.AssignWorkItem;

public record AssignWorkItemCommand(Guid WorkItemId, Guid AssigneeId)
    : IRequest<ErrorOr<Success>>;