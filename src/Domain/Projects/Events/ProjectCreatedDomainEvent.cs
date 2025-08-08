using Domain.Shared;

namespace Domain.Projects.Events;

public sealed record ProjectCreatedDomainEvent(Project Project) : IDomainEvent;