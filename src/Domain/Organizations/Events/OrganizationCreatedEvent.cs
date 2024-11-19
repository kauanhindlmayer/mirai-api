using Domain.Common;

namespace Domain.Organizations.Events;

public record OrganizationCreatedEvent(Guid Id) : IDomainEvent;