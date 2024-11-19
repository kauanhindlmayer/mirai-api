using Domain.Common;

namespace Domain.Organizations.Events;

public record OrganizationDeletedEvent(Guid Id) : IDomainEvent;