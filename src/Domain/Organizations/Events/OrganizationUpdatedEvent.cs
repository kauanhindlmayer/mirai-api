using Domain.Common;

namespace Domain.Organizations.Events;

public record OrganizationUpdatedEvent(Guid Id) : IDomainEvent;