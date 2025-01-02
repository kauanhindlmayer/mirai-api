using Application.Common.Interfaces.Persistence;
using Domain.Boards;
using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.CreateColumn;

internal sealed class CreateColumnCommandHandler(IBoardsRepository boardsRepository)
    : IRequestHandler<CreateColumnCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> Handle(
        CreateColumnCommand command,
        CancellationToken cancellationToken)
    {
        var board = await boardsRepository.GetByIdWithColumnsAsync(
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

        boardsRepository.Update(board);

        return column.Id;
    }
}