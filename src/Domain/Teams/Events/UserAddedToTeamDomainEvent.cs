using Domain.Shared;
using Domain.Users;

namespace Domain.Teams.Events;

public sealed record UserAddedToTeamDomainEvent(
    Team Team,
    User User) : IDomainEvent;