using Application.Common.Interfaces.Persistence;
using Domain.Boards;
using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.DeleteColumn;

internal sealed class DeleteColumnCommandHandler(IBoardsRepository boardsRepository)
    : IRequestHandler<DeleteColumnCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        DeleteColumnCommand command,
        CancellationToken cancellationToken)
    {
        var board = await boardsRepository.GetByIdWithColumnsAsync(
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

        boardsRepository.Update(board);

        return Result.Success;
    }
}