using Domain.Boards;
using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.CreateBoard;

public sealed record CreateBoardCommand(
    Guid ProjectId,
    string Name,
    string Description) : IRequest<ErrorOr<Guid>>;