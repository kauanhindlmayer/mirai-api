using Application.Common.Interfaces;
using Domain.Boards;
using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.AddColumn;

public class AddColumnCommandHandler(IBoardsRepository _boardsRepository)
    : IRequestHandler<AddColumnCommand, ErrorOr<BoardColumn>>
{
    public async Task<ErrorOr<BoardColumn>> Handle(
        AddColumnCommand request,
        CancellationToken cancellationToken)
    {
        var board = await _boardsRepository.GetByIdAsync(
            request.BoardId,
            cancellationToken);

        if (board is null)
        {
            return BoardErrors.BoardNotFound;
        }

        var column = new BoardColumn(
            board.Id,
            request.Name,
            board.Columns.Count,
            request.WipLimit,
            request.DefinitionOfDone);

        var result = board.AddColumn(column);
        if (result.IsError)
        {
            return result.Errors;
        }

        _boardsRepository.Update(board);

        return result.Value;
    }
}