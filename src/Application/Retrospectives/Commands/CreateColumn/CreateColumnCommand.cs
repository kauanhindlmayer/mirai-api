using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.CreateColumn;

public record CreateColumnCommand(string Title, Guid RetrospectiveId)
    : IRequest<ErrorOr<Retrospective>>;