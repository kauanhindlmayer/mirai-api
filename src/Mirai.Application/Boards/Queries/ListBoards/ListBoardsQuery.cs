using ErrorOr;
using MediatR;
using Mirai.Domain.Boards;

namespace Mirai.Application.Boards.Queries.ListBoards;

public record ListBoardsQuery(Guid ProjectId) : IRequest<ErrorOr<List<Board>>>;