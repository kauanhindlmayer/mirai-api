using ErrorOr;
using MediatR;

namespace Application.Boards.Queries.ListBoards;

public sealed record ListBoardsQuery(Guid ProjectId)
    : IRequest<ErrorOr<IReadOnlyList<BoardBriefResponse>>>;