using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.MoveCard;

public record MoveCardCommand(
    Guid BoardId,
    Guid ColumnId,
    Guid CardId,
    Guid TargetColumnId,
    int TargetPosition) : IRequest<ErrorOr<Success>>;