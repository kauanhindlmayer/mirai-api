using ErrorOr;
using MediatR;
using Mirai.Domain.Boards;

namespace Mirai.Application.Boards.Queries.GetBoard;

public record GetBoardQuery(Guid BoardId) : IRequest<ErrorOr<Board>>;