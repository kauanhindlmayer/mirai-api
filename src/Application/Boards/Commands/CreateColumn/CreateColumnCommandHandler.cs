using Application.Common.Interfaces.Persistence;
using Domain.Boards;
using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.CreateColumn;

internal sealed class CreateColumnCommandHandler(IBoardsRepository _boardsRepository)
    : IRequestHandler<CreateColumnCommand, ErrorOr<BoardColumn>>
{
    public async Task<ErrorOr<BoardColumn>> Handle(
        CreateColumnCommand command,
        CancellationToken cancellationToken)
    {
        var board = await _boardsRepository.GetByIdAsync(
            command.BoardId,
            cancellationToken);

        if (board is null)
        {
            return BoardErrors.NotFound;
        }

        var column = new BoardColumn(
            board.Id,
            command.Name,
            command.WipLimit,
            command.DefinitionOfDone);

        var result = board.AddColumn(column);
        if (result.IsError)
        {
            return result.Errors;
        }

        _boardsRepository.Update(board);

        return result.Value;
    }
}