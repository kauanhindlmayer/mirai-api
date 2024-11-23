using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.AssignWorkItem;

public sealed record AssignWorkItemCommand(
    Guid WorkItemId,
    Guid AssigneeId) : IRequest<ErrorOr<Success>>;