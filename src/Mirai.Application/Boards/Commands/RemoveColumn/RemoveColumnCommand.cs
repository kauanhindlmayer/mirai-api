using ErrorOr;
using MediatR;

namespace Mirai.Application.Boards.Commands.RemoveColumn;

public record RemoveColumnCommand(Guid BoardId, Guid ColumnId) : IRequest<ErrorOr<Success>>;