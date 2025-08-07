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
    private readonly ITeamsRepository _teamsRepository;
    private readonly IWikiPagesRepository _wikiPagesRepository;
    private readonly IUserContext _userContext;
    private readonly ISprintsRepository _sprintsRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProjectCreatedDomainEventHandler> _logger;

    public ProjectCreatedDomainEventHandler(
        ITeamsRepository teamsRepository,
        IWikiPagesRepository wikiPagesRepository,
        IUserContext userContext,
        ISprintsRepository sprintsRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork,
        ILogger<ProjectCreatedDomainEventHandler> logger)
    {
        _teamsRepository = teamsRepository;
        _wikiPagesRepository = wikiPagesRepository;
        _userContext = userContext;
        _sprintsRepository = sprintsRepository;
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
            await _teamsRepository.AddAsync(team, cancellationToken);

            var wikiPage = new WikiPage(
                notification.Project.Id,
                "Home",
                "Welcome to the project wiki!",
                _userContext.UserId);
            await _wikiPagesRepository.AddAsync(wikiPage, cancellationToken);

            var sprint = new Sprint(
                team.Id,
                "Sprint 1",
                DateOnly.FromDateTime(_dateTimeProvider.UtcNow),
                DateOnly.FromDateTime(_dateTimeProvider.UtcNow.AddDays(14)));
            await _sprintsRepository.AddAsync(sprint, cancellationToken);

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
