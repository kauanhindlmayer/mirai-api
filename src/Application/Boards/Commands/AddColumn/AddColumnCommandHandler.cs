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

        var result = board.AddColumn(
            request.Name,
            request.WipLimit,
            request.DefinitionOfDone);

        if (result.IsError)
        {
            return result.Errors;
        }

        _boardsRepository.Update(board);

        return result.Value;
    }
}