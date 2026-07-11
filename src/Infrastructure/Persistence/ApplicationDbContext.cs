using Application.Abstractions;
using Application.Abstractions.Authentication;
using Domain.Authorization;
using Domain.Boards;
using Domain.Organizations;
using Domain.Personas;
using Domain.Projects;
using Domain.Retrospectives;
using Domain.Shared;
using Domain.Sprints;
using Domain.TagImportJobs;
using Domain.Tags;
using Domain.Teams;
using Domain.Users;
using Domain.WikiPages;
using Domain.WorkItems;
using Domain.WorkItems.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class ApplicationDbContext(
    DbContextOptions options,
    IPublisher publisher,
    IUserContext userContext) : DbContext(options), IApplicationDbContext
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

    public DbSet<WorkItemChangeSet> WorkItemChangeSets { get; init; }

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

    public DbSet<Role> Roles { get; init; }

    private static readonly Dictionary<string, string> TrackedScalarFields = new()
    {
        [nameof(WorkItem.Title)] = "Title",
        [nameof(WorkItem.Description)] = "Description",
        [nameof(WorkItem.AcceptanceCriteria)] = "Acceptance Criteria",
        [nameof(WorkItem.Type)] = "Type",
        [nameof(WorkItem.Status)] = "Status",
        [nameof(WorkItem.AssigneeId)] = "Assignee",
        [nameof(WorkItem.AssignedTeamId)] = "Assigned Team",
        [nameof(WorkItem.ParentWorkItemId)] = "Parent Work Item",
        [nameof(WorkItem.SprintId)] = "Sprint",
    };

    private static readonly Dictionary<string, string> TrackedComplexFields = new()
    {
        [nameof(Planning.StoryPoints)] = "Story Points",
        [nameof(Planning.Priority)] = "Priority",
        [nameof(Classification.ValueArea)] = "Value Area",
    };

    private static readonly HashSet<string> ReferenceIdFields =
    [
        nameof(WorkItem.AssigneeId),
        nameof(WorkItem.AssignedTeamId),
        nameof(WorkItem.ParentWorkItemId),
        nameof(WorkItem.SprintId),
    ];

    public async override Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        await CaptureWorkItemChangesAsync(cancellationToken);

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

    private void UpdateTimestamps()
    {
        var entities = ChangeTracker.Entries<Entity>()
            .Where(e => e.State == EntityState.Modified)
            .Select(e => e.Entity);

        foreach (var entity in entities)
        {
            typeof(Entity)
                .GetProperty(nameof(Entity.UpdatedAtUtc))!
                .SetValue(entity, DateTime.UtcNow);
        }
    }

    private async Task PublishDomainEvents(List<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent);
        }
    }

    private async Task CaptureWorkItemChangesAsync(CancellationToken cancellationToken)
    {
        var modifiedWorkItems = ChangeTracker.Entries<WorkItem>()
            .Where(entry => entry.State == EntityState.Modified)
            .ToList();

        foreach (var entry in modifiedWorkItems)
        {
            var changeSet = new WorkItemChangeSet(entry.Entity.Id, userContext.UserId);

            foreach (var property in entry.Properties.Where(p => p.IsModified))
            {
                if (!TrackedScalarFields.TryGetValue(property.Metadata.Name, out var fieldName))
                {
                    continue;
                }

                var oldValue = await FormatScalarValueAsync(
                    property.Metadata.Name,
                    property.OriginalValue,
                    cancellationToken);
                var newValue = await FormatScalarValueAsync(
                    property.Metadata.Name,
                    property.CurrentValue,
                    cancellationToken);

                if (oldValue == newValue)
                {
                    continue;
                }

                changeSet.AddChange(fieldName, oldValue, newValue);
            }

            foreach (var complexProperty in entry.ComplexProperties)
            {
                foreach (var property in complexProperty.Properties.Where(p => p.IsModified))
                {
                    if (!TrackedComplexFields.TryGetValue(property.Metadata.Name, out var fieldName))
                    {
                        continue;
                    }

                    if (property.OriginalValue?.ToString() == property.CurrentValue?.ToString())
                    {
                        continue;
                    }

                    changeSet.AddChange(
                        fieldName,
                        property.OriginalValue?.ToString(),
                        property.CurrentValue?.ToString());
                }
            }

            if (changeSet.Changes.Count > 0)
            {
                WorkItemChangeSets.Add(changeSet);
            }
        }
    }

    private async Task<string?> FormatScalarValueAsync(
        string propertyName,
        object? value,
        CancellationToken cancellationToken)
    {
        if (value is null)
        {
            return null;
        }

        if (!ReferenceIdFields.Contains(propertyName))
        {
            return value.ToString();
        }

        var id = (Guid)value;
        return propertyName switch
        {
            nameof(WorkItem.AssigneeId) => await Users
                .Where(u => u.Id == id)
                .Select(u => u.FirstName + " " + u.LastName)
                .FirstOrDefaultAsync(cancellationToken),
            nameof(WorkItem.AssignedTeamId) => await Teams
                .Where(t => t.Id == id)
                .Select(t => t.Name)
                .FirstOrDefaultAsync(cancellationToken),
            nameof(WorkItem.ParentWorkItemId) => await WorkItems
                .Where(w => w.Id == id)
                .Select(w => w.Title)
                .FirstOrDefaultAsync(cancellationToken),
            nameof(WorkItem.SprintId) => await Sprints
                .Where(s => s.Id == id)
                .Select(s => s.Name)
                .FirstOrDefaultAsync(cancellationToken),
            _ => id.ToString(),
        };
    }
}