using Application.Common.Interfaces.Persistence;
using Domain.Boards;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.CreateCard;

internal sealed class CreateCardCommandHandler(
    IWorkItemsRepository _workItemsRepository,
    IBoardsRepository _boardRepository)
    : IRequestHandler<CreateCardCommand, ErrorOr<BoardCard>>
{
    public async Task<ErrorOr<BoardCard>> Handle(
        CreateCardCommand command,
        CancellationToken cancellationToken)
    {
        var board = await _boardRepository.GetByIdAsync(
            command.BoardId,
            cancellationToken);

        if (board is null)
        {
            return BoardErrors.NotFound;
        }

        var workItemCode = await _workItemsRepository.GetNextWorkItemCodeAsync(
            board.ProjectId,
            cancellationToken);

        var workItem = new WorkItem(
            board.ProjectId,
            workItemCode,
            command.Title,
            command.Type);

        await _workItemsRepository.AddAsync(workItem, cancellationToken);

        var column = board.Columns.SingleOrDefault(c => c.Id == command.ColumnId);
        if (column is null)
        {
            return BoardErrors.ColumnNotFound;
        }

        var card = column.AddCard(workItem);
        _boardRepository.Update(board);

        return card;
    }
}