using Application.Common.Interfaces.Persistence;
using Domain.Boards;
using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.DeleteColumn;

internal sealed class DeleteColumnCommandHandler(IBoardsRepository _boardsRepository)
    : IRequestHandler<DeleteColumnCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        DeleteColumnCommand command,
        CancellationToken cancellationToken)
    {
        var board = await _boardsRepository.GetByIdAsync(
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

        _boardsRepository.Update(board);

        return Result.Success;
    }
}