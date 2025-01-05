using Domain.Common;

namespace Domain.Teams.Events;

public sealed record TeamCreatedDomainEvent(
    Guid Id,
    string Name,
    Guid ProjectId) : IDomainEvent;