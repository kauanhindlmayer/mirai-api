using Domain.Common;

namespace Domain.Organizations.Events;

public sealed record OrganizationUpdatedDomainEvent(Guid Id) : IDomainEvent;