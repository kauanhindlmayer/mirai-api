using ErrorOr;
using MediatR;
using Mirai.Domain.Retrospectives;

namespace Mirai.Application.Retrospectives.Commands.AddColumn;

public record AddColumnCommand(string Title, Guid RetrospectiveId)
    : IRequest<ErrorOr<Retrospective>>;