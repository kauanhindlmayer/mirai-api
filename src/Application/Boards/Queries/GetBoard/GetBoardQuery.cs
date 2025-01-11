using Domain.WorkItems.Enums;
using ErrorOr;
using MediatR;

namespace Application.Boards.Queries.GetBoard;

public sealed record GetBoardQuery(Guid BoardId) : IRequest<ErrorOr<BoardResponse>>;