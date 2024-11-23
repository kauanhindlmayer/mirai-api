using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.DeleteBoard;

public sealed record DeleteBoardCommand(Guid BoardId) : IRequest<ErrorOr<Success>>;