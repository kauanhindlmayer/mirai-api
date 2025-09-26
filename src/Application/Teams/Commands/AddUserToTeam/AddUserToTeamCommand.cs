using ErrorOr;
using MediatR;

namespace Application.Teams.Commands.AddUserToTeam;

public sealed record AddUserToTeamCommand(
    Guid TeamId,
    Guid UserId) : IRequest<ErrorOr<Success>>;