using Application.Common.Interfaces;
using Domain.Boards;
using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.RemoveColumn;

public class RemoveColumnCommandHandler(IBoardsRepository _boardsRepository)
    : IRequestHandler<RemoveColumnCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(
        RemoveColumnCommand request,
        CancellationToken cancellationToken)
    {
        var board = await _boardsRepository.GetByIdAsync(
            request.BoardId,
            cancellationToken);

        if (board is null)
        {
            return BoardErrors.BoardNotFound;
        }

        var result = board.RemoveColumn(request.ColumnId);
        if (result.IsError)
        {
            return result.Errors;
        }

        _boardsRepository.Update(board);

        return Result.Success;
    }
}