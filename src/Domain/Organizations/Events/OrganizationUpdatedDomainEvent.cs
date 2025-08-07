using Domain.Shared;

namespace Domain.Organizations.Events;

public sealed record OrganizationUpdatedDomainEvent(Organization Organization) : IDomainEvent;