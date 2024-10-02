using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.DeleteColumn;

public record DeleteColumnCommand(Guid BoardId, Guid ColumnId) : IRequest<ErrorOr<Success>>;