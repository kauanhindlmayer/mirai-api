using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Domain.Projects.Events;
using Domain.Teams;
using Domain.WikiPages;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Projects.Events;

internal sealed class ProjectCreatedDomainEventHandler : INotificationHandler<ProjectCreatedDomainEvent>
{
    private readonly ITeamsRepository _teamsRepository;
    private readonly IWikiPagesRepository _wikiPagesRepository;
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProjectCreatedDomainEventHandler> _logger;

    public ProjectCreatedDomainEventHandler(
        ITeamsRepository teamsRepository,
        IWikiPagesRepository wikiPagesRepository,
        IUserContext userContext,
        IUnitOfWork unitOfWork,
        ILogger<ProjectCreatedDomainEventHandler> logger)
    {
        _teamsRepository = teamsRepository;
        _wikiPagesRepository = wikiPagesRepository;
        _userContext = userContext;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(ProjectCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var team = new Team(
            notification.Project.Id,
            notification.Project.Name,
            "The default project team.");
        await _teamsRepository.AddAsync(team, cancellationToken);

        var wikiPage = new WikiPage(
            notification.Project.Id,
            "Home",
            "Welcome to the project wiki!",
            _userContext.UserId);
        await _wikiPagesRepository.AddAsync(wikiPage, cancellationToken);

        _logger.LogInformation(
            "Default team and wiki page created for project {ProjectName}.",
            notification.Project.Name);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
