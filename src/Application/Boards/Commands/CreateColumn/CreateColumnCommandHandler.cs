using Domain.Boards;
using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.CreateColumn;

internal sealed class CreateColumnCommandHandler : IRequestHandler<CreateColumnCommand, ErrorOr<Guid>>
{
    private readonly IBoardsRepository _boardsRepository;

    public CreateColumnCommandHandler(IBoardsRepository boardsRepository)
    {
        _boardsRepository = boardsRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(
        CreateColumnCommand command,
        CancellationToken cancellationToken)
    {
        var board = await _boardsRepository.GetByIdWithColumnsAsync(
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

        var result = board.AddColumnAtPosition(column, command.Position);
        if (result.IsError)
        {
            return result.Errors;
        }

        _boardsRepository.Update(board);

        return column.Id;
    }
}