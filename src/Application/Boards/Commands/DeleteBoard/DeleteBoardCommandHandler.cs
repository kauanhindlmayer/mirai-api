using Domain.Boards;
using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.DeleteBoard;

internal sealed class DeleteBoardCommandHandler
    : IRequestHandler<DeleteBoardCommand, ErrorOr<Success>>
{
    private readonly IBoardRepository _boardRepository;

    public DeleteBoardCommandHandler(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        DeleteBoardCommand command,
        CancellationToken cancellationToken)
    {
        var board = await _boardRepository.GetByIdAsync(
            command.BoardId,
            cancellationToken);

        if (board is null)
        {
            return BoardErrors.NotFound;
        }

        _boardRepository.Remove(board);

        return Result.Success;
    }
}