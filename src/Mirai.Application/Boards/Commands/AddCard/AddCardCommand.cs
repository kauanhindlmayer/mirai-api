using ErrorOr;
using MediatR;
using Mirai.Domain.Boards;
using Mirai.Domain.WorkItems.Enums;

namespace Mirai.Application.Boards.Commands.AddCard;

public record AddCardCommand(
    Guid BoardId,
    Guid ColumnId,
    WorkItemType Type,
    string Title) : IRequest<ErrorOr<BoardCard>>;