using Domain.Shared;
using Domain.Users;

namespace Domain.Teams.Events;

public sealed record UserRemovedFromTeamDomainEvent(
    Team Team,
    User User) : IDomainEvent;