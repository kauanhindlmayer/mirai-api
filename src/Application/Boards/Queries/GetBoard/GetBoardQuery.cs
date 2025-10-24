using Domain.Backlogs;
using ErrorOr;
using MediatR;

namespace Application.Boards.Queries.GetBoard;

public sealed record GetBoardQuery(
    Guid BoardId,
    BacklogLevel? BacklogLevel = null,
    int PageSize = 20) : IRequest<ErrorOr<BoardResponse>>;