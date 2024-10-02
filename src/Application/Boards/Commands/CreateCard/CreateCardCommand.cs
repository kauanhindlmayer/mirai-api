using Domain.Boards;
using Domain.WorkItems.Enums;
using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.CreateCard;

public record CreateCardCommand(
    Guid BoardId,
    Guid ColumnId,
    WorkItemType Type,
    string Title) : IRequest<ErrorOr<BoardCard>>;