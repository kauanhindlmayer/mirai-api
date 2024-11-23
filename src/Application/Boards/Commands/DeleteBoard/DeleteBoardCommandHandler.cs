using Application.Common.Interfaces.Persistence;
using Domain.Boards;
using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.DeleteBoard;

internal sealed class DeleteBoardCommandHandler(IBoardsRepository boardRepository)
    : IRequestHandler<DeleteBoardCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        DeleteBoardCommand command,
        CancellationToken cancellationToken)
    {
        var board = await boardRepository.GetByIdAsync(
            command.BoardId,
            cancellationToken);

        if (board is null)
        {
            return BoardErrors.NotFound;
        }

        boardRepository.Remove(board);

        return Result.Success;
    }
}