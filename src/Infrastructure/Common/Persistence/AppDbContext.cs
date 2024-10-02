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
using Infrastructure.Common.Middleware;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common.Persistence;

public class AppDbContext(
    DbContextOptions options,
    IHttpContextAccessor _httpContextAccessor,
    IPublisher _publisher) : DbContext(options)
{
    public DbSet<User> Users { get; init; }

    public DbSet<Organization> Organizations { get; init; }

    public DbSet<Project> Projects { get; init; }

    public DbSet<Team> Teams { get; init; }

    public DbSet<Board> Boards { get; init; }

    public DbSet<BoardColumn> BoardColumns { get; init; }

    public DbSet<BoardCard> BoardCards { get; init; }

    public DbSet<WorkItem> WorkItems { get; init; }

    public DbSet<WorkItemComment> WorkItemComments { get; init; }

    public DbSet<WikiPage> WikiPages { get; init; }

    public DbSet<WikiPageComment> WikiPageComments { get; init; }

    public DbSet<Retrospective> Retrospectives { get; init; }

    public DbSet<RetrospectiveColumn> RetrospectiveColumns { get; init; }

    public DbSet<RetrospectiveItem> RetrospectiveItems { get; init; }

    public DbSet<Tag> Tags { get; init; }

    public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = ChangeTracker.Entries<Entity>()
           .SelectMany(entry => entry.Entity.PopDomainEvents())
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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
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