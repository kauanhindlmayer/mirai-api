using Domain.Shared;

namespace Domain.Organizations.Events;

public sealed record OrganizationCreatedDomainEvent(Organization Organization) : IDomainEvent;