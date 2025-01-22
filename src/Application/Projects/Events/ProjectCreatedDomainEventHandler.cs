using Application.Common.Interfaces.Persistence;
using Domain.Projects.Events;
using Domain.Teams;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Projects.Events;

internal sealed class ProjectCreatedDomainEventHandler : INotificationHandler<ProjectCreatedDomainEvent>
{
    private readonly ITeamsRepository _teamsRepository;
    private readonly ILogger<ProjectCreatedDomainEventHandler> _logger;

    public ProjectCreatedDomainEventHandler(
        ITeamsRepository boardsRepository,
        ILogger<ProjectCreatedDomainEventHandler> logger)
    {
        _teamsRepository = boardsRepository;
        _logger = logger;
    }

    public async Task Handle(
        ProjectCreatedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        var team = new Team(
            notification.Project.Id,
            notification.Project.Name,
            "The default project team.");

        await _teamsRepository.AddAsync(team, cancellationToken);
        _logger.LogInformation("Team created for project {ProjectName}", notification.Project.Name);
    }
}
