using Domain.Shared;

namespace Domain.WorkItems.Events;

public sealed record PullRequestLinksUpdatedDomainEvent(WorkItem WorkItem) : IDomainEvent;
