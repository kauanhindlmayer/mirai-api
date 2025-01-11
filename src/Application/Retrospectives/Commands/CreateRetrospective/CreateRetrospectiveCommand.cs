using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.CreateRetrospective;

public sealed record CreateRetrospectiveCommand(
    string Title,
    string Description,
    Guid TeamId) : IRequest<ErrorOr<Guid>>;