using Domain.Common;

namespace Domain.Organizations.Events;

public sealed record OrganizationDeletedDomainEvent(Organization Organization) : IDomainEvent;