using Application.Common.Interfaces.Persistence;
using Domain.Boards;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.Boards.Commands.CreateCard;

internal sealed class CreateCardCommandHandler : IRequestHandler<CreateCardCommand, ErrorOr<Guid>>
{
    private readonly IWorkItemsRepository _workItemsRepository;
    private readonly IBoardsRepository _boardsRepository;

    public CreateCardCommandHandler(
        IWorkItemsRepository workItemsRepository,
        IBoardsRepository boardsRepository)
    {
        _workItemsRepository = workItemsRepository;
        _boardsRepository = boardsRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(
        CreateCardCommand command,
        CancellationToken cancellationToken)
    {
        var board = await _boardsRepository.GetByIdWithCardsAsync(
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

        var result = column.AddCard(workItem);
        if (result.IsError)
        {
            return result.Errors;
        }

        _boardsRepository.Update(board);

        return result.Value.Id;
    }
}