using Domain.Boards;
using Domain.Teams.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Teams.Events;

internal sealed class TeamCreatedDomainEventHandler : INotificationHandler<TeamCreatedDomainEvent>
{
    private readonly IBoardRepository _boardRepository;
    private readonly ILogger<TeamCreatedDomainEventHandler> _logger;

    public TeamCreatedDomainEventHandler(
        IBoardRepository boardRepository,
        ILogger<TeamCreatedDomainEventHandler> logger)
    {
        _boardRepository = boardRepository;
        _logger = logger;
    }

    public async Task Handle(
        TeamCreatedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        var board = new Board(
            notification.Team.Id,
            notification.Team.Name);

        await _boardRepository.AddAsync(board, cancellationToken);

        _logger.LogInformation(
            "Board created for team {TeamName}",
            notification.Team.Name);
    }
}
