using ErrorOr;
using MediatR;

namespace Application.Sprints.Commands.AddWorkItemToSprint;

public sealed record AddWorkItemToSprintCommand(
    Guid TeamId,
    Guid SprintId,
    Guid WorkItemId) : IRequest<ErrorOr<Success>>;