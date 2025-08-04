using Domain.Common;
using Domain.Users;

namespace Domain.Organizations.Events;

public sealed record UserAddedToOrganizationDomainEvent(
    Organization Organization,
    User User) : IDomainEvent;