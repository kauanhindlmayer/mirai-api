using ErrorOr;
using MediatR;

namespace Mirai.Application.Boards.Commands.DeleteBoard;

public record DeleteBoardCommand(Guid BoardId) : IRequest<ErrorOr<Success>>;