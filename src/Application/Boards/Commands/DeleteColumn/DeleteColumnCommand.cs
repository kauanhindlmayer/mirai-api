using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.DeleteColumn;

public sealed record DeleteColumnCommand(
    Guid BoardId,
    Guid ColumnId) : IRequest<ErrorOr<Success>>;