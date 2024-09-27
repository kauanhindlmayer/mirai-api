using ErrorOr;
using MediatR;
using Mirai.Domain.Boards;

namespace Mirai.Application.Boards.Commands.CreateBoard;

public record CreateBoardCommand(
    Guid ProjectId,
    string Name,
    string Description) : IRequest<ErrorOr<Board>>;