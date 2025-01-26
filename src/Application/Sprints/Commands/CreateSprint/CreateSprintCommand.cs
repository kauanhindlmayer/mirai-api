using ErrorOr;
using MediatR;

namespace Application.Sprints.Commands.CreateSprint;

public sealed record CreateSprintCommand(
    Guid TeamId,
    string Name,
    DateTime StartDate,
    DateTime EndDate) : IRequest<ErrorOr<Guid>>;