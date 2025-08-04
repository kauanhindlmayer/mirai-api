using Domain.Common;
using Domain.Users;

namespace Domain.Projects.Events;

public sealed record UserRemovedFromProjectDomainEvent(
    Project Project,
    User User) : IDomainEvent;