using ErrorOr;
using MediatR;

namespace Application.Boards.Queries.GetBoard;

public sealed record GetBoardQuery(
    Guid TeamId,
    Guid BoardId) : IRequest<ErrorOr<BoardResponse>>;