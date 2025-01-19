using Application.Common.Interfaces.Persistence;
using Domain.Boards;
using Domain.Teams.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Teams.Events;

internal sealed class TeamCreatedDomainEventHandler : INotificationHandler<TeamCreatedDomainEvent>
{
    private readonly IBoardsRepository _boardsRepository;
    private readonly ILogger<TeamCreatedDomainEventHandler> _logger;

    public TeamCreatedDomainEventHandler(
        IBoardsRepository boardsRepository,
        ILogger<TeamCreatedDomainEventHandler> logger)
    {
        _boardsRepository = boardsRepository;
        _logger = logger;
    }

    public async Task Handle(TeamCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var board = new Board(
            notification.ProjectId,
            notification.Name,
            string.Empty);

        await _boardsRepository.AddAsync(board, cancellationToken);
        _logger.LogInformation("Board created for team {TeamId}", notification.Id);
    }
}
