using Domain.Shared;
using Domain.Users;

namespace Domain.Projects.Events;

public sealed record UserAddedToProjectDomainEvent(
    Project Project,
    User User) : IDomainEvent;