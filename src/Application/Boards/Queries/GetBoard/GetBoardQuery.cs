using Domain.Boards;
using ErrorOr;
using MediatR;

namespace Application.Boards.Queries.GetBoard;

public record GetBoardQuery(Guid BoardId) : IRequest<ErrorOr<Board>>;