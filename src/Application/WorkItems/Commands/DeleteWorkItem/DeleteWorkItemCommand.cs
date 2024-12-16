using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.DeleteWorkItem;

public sealed record DeleteWorkItemCommand(
    Guid WorkItemId) : IRequest<ErrorOr<Success>>;