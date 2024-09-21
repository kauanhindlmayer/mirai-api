using ErrorOr;
using MediatR;
using Mirai.Domain.Retrospectives;

namespace Mirai.Application.Retrospectives.Commands.AddItem;

public record AddItemCommand(
    string Description,
    Guid RetrospectiveId,
    Guid RetrospectiveColumnId)
    : IRequest<ErrorOr<Retrospective>>;