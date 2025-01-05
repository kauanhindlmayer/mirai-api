using Application.Common.Interfaces.Persistence;
using Domain.Boards;
using Domain.Teams.Events;
using MediatR;

namespace Application.Teams.Events;

internal sealed class TeamCreatedDomainEventHandler(IBoardsRepository boardRepository)
    : INotificationHandler<TeamCreatedDomainEvent>
{
    public async Task Handle(
        TeamCreatedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        var board = new Board(
            notification.ProjectId,
            notification.Name,
            string.Empty);

        await boardRepository.AddAsync(board, cancellationToken);
    }
}
