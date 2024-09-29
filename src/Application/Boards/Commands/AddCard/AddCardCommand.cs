using Domain.Boards;
using Domain.WorkItems.Enums;
using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.AddCard;

public record AddCardCommand(
    Guid BoardId,
    Guid ColumnId,
    WorkItemType Type,
    string Title) : IRequest<ErrorOr<BoardCard>>;