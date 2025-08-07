using Domain.Boards;
using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.DeleteBoard;

internal sealed class DeleteBoardCommandHandler
    : IRequestHandler<DeleteBoardCommand, ErrorOr<Success>>
{
    private readonly IBoardsRepository _boardsRepository;

    public DeleteBoardCommandHandler(IBoardsRepository boardsRepository)
    {
        _boardsRepository = boardsRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        DeleteBoardCommand command,
        CancellationToken cancellationToken)
    {
        var board = await _boardsRepository.GetByIdAsync(
            command.BoardId,
            cancellationToken);

        if (board is null)
        {
            return BoardErrors.NotFound;
        }

        _boardsRepository.Remove(board);

        return Result.Success;
    }
}