using Domain.Retrospectives.Enums;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.CreateRetrospective;

public sealed record CreateRetrospectiveCommand(
    string Title,
    string Description,
    RetrospectiveTemplate? Template,
    Guid TeamId) : IRequest<ErrorOr<Guid>>;