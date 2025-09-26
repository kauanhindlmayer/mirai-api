using Application.Abstractions.Authentication;
using Application.Abstractions.Time;
using Domain.Projects.Events;
using Domain.Shared;
using Domain.Sprints;
using Domain.Teams;
using Domain.WikiPages;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Projects.Events;

internal sealed class ProjectCreatedDomainEventHandler
    : INotificationHandler<ProjectCreatedDomainEvent>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IWikiPageRepository _wikiPageRepository;
    private readonly IUserContext _userContext;
    private readonly ISprintRepository _sprintRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProjectCreatedDomainEventHandler> _logger;

    public ProjectCreatedDomainEventHandler(
        ITeamRepository teamRepository,
        IWikiPageRepository wikiPageRepository,
        IUserContext userContext,
        ISprintRepository sprintRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork,
        ILogger<ProjectCreatedDomainEventHandler> logger)
    {
        _teamRepository = teamRepository;
        _wikiPageRepository = wikiPageRepository;
        _userContext = userContext;
        _sprintRepository = sprintRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(ProjectCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var team = new Team(
                notification.Project.Id,
                notification.Project.Name,
                "The default project team.");
            await _teamRepository.AddAsync(team, cancellationToken);

            var wikiPage = new WikiPage(
                notification.Project.Id,
                "Home",
                "Welcome to the project wiki!",
                _userContext.UserId);
            await _wikiPageRepository.AddAsync(wikiPage, cancellationToken);

            var sprint = new Sprint(
                team.Id,
                "Sprint 1",
                DateOnly.FromDateTime(_dateTimeProvider.UtcNow),
                DateOnly.FromDateTime(_dateTimeProvider.UtcNow.AddDays(14)));
            await _sprintRepository.AddAsync(sprint, cancellationToken);

            _logger.LogInformation(
                "Default team, wiki page, and sprint created for project {ProjectName}.",
                notification.Project.Name);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An error occurred while creating default team, wiki page, and sprint for project {ProjectName}.",
                notification.Project.Name);
        }
    }
}
