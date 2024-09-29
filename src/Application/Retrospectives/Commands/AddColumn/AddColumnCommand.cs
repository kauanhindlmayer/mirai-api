using Domain.Retrospectives;
using ErrorOr;
using MediatR;

namespace Application.Retrospectives.Commands.AddColumn;

public record AddColumnCommand(string Title, Guid RetrospectiveId)
    : IRequest<ErrorOr<Retrospective>>;