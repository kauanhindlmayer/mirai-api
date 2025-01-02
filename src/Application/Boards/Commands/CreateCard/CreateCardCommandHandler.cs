using Application.Common.Interfaces.Persistence;
using Domain.Boards;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.CreateCard;

internal sealed class CreateCardCommandHandler(
    IWorkItemsRepository workItemsRepository,
    IBoardsRepository boardRepository)
    : IRequestHandler<CreateCardCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> Handle(
        CreateCardCommand command,
        CancellationToken cancellationToken)
    {
        var board = await boardRepository.GetByIdAsync(
            command.BoardId,
            cancellationToken);

        if (board is null)
        {
            return BoardErrors.NotFound;
        }

        var workItemCode = await workItemsRepository.GetNextWorkItemCodeAsync(
            board.ProjectId,
            cancellationToken);

        var workItem = new WorkItem(
            board.ProjectId,
            workItemCode,
            command.Title,
            command.Type);

        await workItemsRepository.AddAsync(workItem, cancellationToken);

        var column = board.Columns.SingleOrDefault(c => c.Id == command.ColumnId);
        if (column is null)
        {
            return BoardErrors.ColumnNotFound;
        }

        var result = column.AddCard(workItem);
        if (result.IsError)
        {
            return result.Errors;
        }

        boardRepository.Update(board);

        return result.Value.Id;
    }
}