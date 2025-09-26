using Domain.Boards;
using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.DeleteColumn;

internal sealed class DeleteColumnCommandHandler
    : IRequestHandler<DeleteColumnCommand, ErrorOr<Success>>
{
    private readonly IBoardRepository _boardRepository;

    public DeleteColumnCommandHandler(IBoardRepository boardRepository)
    {
        _boardRepository = boardRepository;
    }

    public async Task<ErrorOr<Success>> Handle(
        DeleteColumnCommand command,
        CancellationToken cancellationToken)
    {
        var board = await _boardRepository.GetByIdWithColumnsAsync(
            command.BoardId,
            cancellationToken);

        if (board is null)
        {
            return BoardErrors.NotFound;
        }

        var result = board.RemoveColumn(command.ColumnId);
        if (result.IsError)
        {
            return result.Errors;
        }

        _boardRepository.Update(board);

        return Result.Success;
    }
}