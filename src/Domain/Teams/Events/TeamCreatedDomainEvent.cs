using Domain.Common;

namespace Domain.Teams.Events;

public sealed record TeamCreatedDomainEvent(Team Team) : IDomainEvent;