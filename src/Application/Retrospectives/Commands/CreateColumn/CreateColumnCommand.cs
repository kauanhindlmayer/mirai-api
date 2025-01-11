using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.CreateColumn;

public sealed record CreateColumnCommand(
    string Title,
    Guid RetrospectiveId) : IRequest<ErrorOr<Guid>>;