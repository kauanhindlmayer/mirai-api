using Domain.Common;

namespace Domain.Organizations.Events;

public sealed record OrganizationCreatedDomainEvent(Organization Organization) : IDomainEvent;