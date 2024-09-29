using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.CreateRetrospective;

public record CreateRetrospectiveCommand(
    string Title,
    string Description,
    Guid TeamId) : IRequest<ErrorOr<Retrospective>>;