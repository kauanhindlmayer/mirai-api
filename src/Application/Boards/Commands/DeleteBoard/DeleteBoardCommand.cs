using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.DeleteBoard;

public record DeleteBoardCommand(Guid BoardId) : IRequest<ErrorOr<Success>>;