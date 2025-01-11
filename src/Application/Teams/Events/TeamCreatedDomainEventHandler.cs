using Application.Common.Interfaces.Persistence;
using Domain.Boards;
using Domain.Teams.Events;
using MediatR;

namespace Application.Teams.Events;

internal sealed class TeamCreatedDomainEventHandler : INotificationHandler<TeamCreatedDomainEvent>
{
    private readonly IBoardsRepository _boardsRepository;

    public TeamCreatedDomainEventHandler(IBoardsRepository boardsRepository)
    {
        _boardsRepository = boardsRepository;
    }

    public async Task Handle(TeamCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var board = new Board(
            notification.ProjectId,
            notification.Name,
            string.Empty);

        await _boardsRepository.AddAsync(board, cancellationToken);
    }
}
