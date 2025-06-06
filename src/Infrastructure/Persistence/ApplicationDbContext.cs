using Application.Common.Interfaces.Persistence;
using Domain.Boards;
using Domain.Common;
using Domain.Organizations;
using Domain.Personas;
using Domain.Projects;
using Domain.Retrospectives;
using Domain.Sprints;
using Domain.TagImportJobs;
using Domain.Tags;
using Domain.Teams;
using Domain.Users;
using Domain.WikiPages;
using Domain.WorkItems;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class ApplicationDbContext(
    DbContextOptions options,
    IPublisher publisher) : DbContext(options), IApplicationDbContext
{
    private const string PostgresVectorExtension = "vector";

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

    public DbSet<WikiPageView> WikiPageViews { get; init; }

    public DbSet<Retrospective> Retrospectives { get; init; }

    public DbSet<RetrospectiveColumn> RetrospectiveColumns { get; init; }

    public DbSet<RetrospectiveItem> RetrospectiveItems { get; init; }

    public DbSet<Tag> Tags { get; init; }

    public DbSet<TagImportJob> TagImportJobs { get; init; }

    public DbSet<Sprint> Sprints { get; init; }

    public DbSet<Persona> Personas { get; init; }

    public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = ChangeTracker.Entries<Entity>()
           .SelectMany(entry =>
            {
                var domainEvents = entry.Entity.GetDomainEvents();
                entry.Entity.ClearDomainEvents();
                return domainEvents;
            })
           .ToList();

        await PublishDomainEvents(domainEvents);
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension(PostgresVectorExtension);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    private async Task PublishDomainEvents(List<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent);
        }
    }
}