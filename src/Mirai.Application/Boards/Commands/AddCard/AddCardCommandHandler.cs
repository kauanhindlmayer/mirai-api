using ErrorOr;
using MediatR;
using Mirai.Application.Common.Interfaces;
using Mirai.Domain.Boards;
using Mirai.Domain.WorkItems;

namespace Mirai.Application.Boards.Commands.AddCard;

public class AddCardCommandHandler(
    IWorkItemsRepository _workItemsRepository,
    IBoardsRepository _boardRepository)
    : IRequestHandler<AddCardCommand, ErrorOr<BoardCard>>
{
    public async Task<ErrorOr<BoardCard>> Handle(AddCardCommand request, CancellationToken cancellationToken)
    {
        var board = await _boardRepository.GetByIdAsync(
            request.BoardId,
            cancellationToken);

        if (board is null)
        {
            return BoardErrors.BoardNotFound;
        }

        var workItemCode = await _workItemsRepository.GetNextWorkItemCodeAsync(
            board.ProjectId,
            cancellationToken);

        var workItem = new WorkItem(
            board.ProjectId,
            workItemCode,
            request.Title,
            request.Type);

        await _workItemsRepository.AddAsync(workItem, cancellationToken);

        var result = board.GetColumn(request.ColumnId);
        if (result.IsError)
        {
            return result.Errors;
        }

        var column = result.Value;
        var card = column.AddCard(workItem);
        _boardRepository.Update(board);

        return card;
    }
}