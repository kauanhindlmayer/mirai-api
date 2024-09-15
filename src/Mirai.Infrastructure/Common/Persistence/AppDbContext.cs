using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Mirai.Domain.Common;
using Mirai.Domain.Organizations;
using Mirai.Domain.Projects;
using Mirai.Domain.Users;
using Mirai.Domain.WikiPages;
using Mirai.Domain.WorkItems;
using Mirai.Infrastructure.Common.Middleware;

namespace Mirai.Infrastructure.Common.Persistence;

public class AppDbContext(
    DbContextOptions options,
    IHttpContextAccessor _httpContextAccessor,
    IPublisher _publisher) : DbContext(options)
{
    public DbSet<User> Users { get; init; }

    public DbSet<Organization> Organizations { get; init; }

    public DbSet<Project> Projects { get; init; }

    public DbSet<WorkItem> WorkItems { get; init; }

    public DbSet<WorkItemComment> WorkItemComments { get; init; }

    public DbSet<WikiPage> WikiPages { get; init; }

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