using Domain.Common;

namespace Domain.Organizations.Events;

public sealed record OrganizationCreatedDomainEvent(Guid Id) : IDomainEvent;