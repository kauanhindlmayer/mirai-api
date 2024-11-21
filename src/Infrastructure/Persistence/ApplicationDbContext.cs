using Domain.Boards;
using Domain.Common;
using Domain.Organizations;
using Domain.Projects;
using Domain.Retrospectives;
using Domain.Tags;
using Domain.Teams;
using Domain.Users;
using Domain.WikiPages;
using Domain.WorkItems;
using Infrastructure.Middlewares;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext(
    DbContextOptions options,
    IHttpContextAccessor _httpContextAccessor,
    IPublisher _publisher) : DbContext(options)
{
    public DbSet<User> Users { get; init; } = null!;

    public DbSet<Organization> Organizations { get; init; } = null!;

    public DbSet<Project> Projects { get; init; } = null!;

    public DbSet<Team> Teams { get; init; } = null!;

    public DbSet<Board> Boards { get; init; } = null!;

    public DbSet<BoardColumn> BoardColumns { get; init; } = null!;

    public DbSet<BoardCard> BoardCards { get; init; } = null!;

    public DbSet<WorkItem> WorkItems { get; init; } = null!;

    public DbSet<WorkItemComment> WorkItemComments { get; init; } = null!;

    public DbSet<WikiPage> WikiPages { get; init; } = null!;

    public DbSet<WikiPageComment> WikiPageComments { get; init; } = null!;

    public DbSet<Retrospective> Retrospectives { get; init; } = null!;

    public DbSet<RetrospectiveColumn> RetrospectiveColumns { get; init; } = null!;

    public DbSet<RetrospectiveItem> RetrospectiveItems { get; init; } = null!;

    public DbSet<Tag> Tags { get; init; } = null!;

    public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = ChangeTracker.Entries<Entity>()
           .SelectMany(entry => entry.Entity.GetDomainEvents())
           .ToList();

        if (IsUserWaitingOnline())
        {
            AddDomainEventsToOfflineProcessingQueue(domainEvents);
            return await base.SaveChangesAsync(cancellationToken);
        }

        await PublishDomainEvents(domainEvents);
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    private bool IsUserWaitingOnline() => _httpContextAccessor.HttpContext is not null;

    private async Task PublishDomainEvents(List<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent);
        }
    }

    private void AddDomainEventsToOfflineProcessingQueue(List<IDomainEvent> domainEvents)
    {
        Queue<IDomainEvent> domainEventsQueue = _httpContextAccessor.HttpContext!.Items.TryGetValue(EventualConsistencyMiddleware.DomainEventsKey, out var value) &&
            value is Queue<IDomainEvent> existingDomainEvents
                ? existingDomainEvents
                : new();

        domainEvents.ForEach(domainEventsQueue.Enqueue);
        _httpContextAccessor.HttpContext.Items[EventualConsistencyMiddleware.DomainEventsKey] = domainEventsQueue;
    }
}