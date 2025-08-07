using Domain.Shared;
using Domain.Users;

namespace Domain.Organizations.Events;

public sealed record UserRemovedFromOrganizationDomainEvent(
    Organization Organization,
    User User) : IDomainEvent;