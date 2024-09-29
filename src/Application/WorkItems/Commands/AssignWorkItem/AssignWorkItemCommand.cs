using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.AssignWorkItem;

public record AssignWorkItemCommand(Guid WorkItemId, Guid AssigneeId)
    : IRequest<ErrorOr<Success>>;