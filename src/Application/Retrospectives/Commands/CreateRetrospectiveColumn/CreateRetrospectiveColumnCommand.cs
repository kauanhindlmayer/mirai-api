using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.CreateRetrospectiveColumn;

public sealed record CreateRetrospectiveColumnCommand(
    string Title,
    Guid RetrospectiveId) : IRequest<ErrorOr<Guid>>;