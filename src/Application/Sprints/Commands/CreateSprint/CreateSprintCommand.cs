using ErrorOr;
using MediatR;

namespace Application.Sprints.Commands.CreateSprint;

public sealed record CreateSprintCommand(
    Guid TeamId,
    string Name,
    DateOnly StartDate,
    DateOnly EndDate) : IRequest<ErrorOr<Guid>>;