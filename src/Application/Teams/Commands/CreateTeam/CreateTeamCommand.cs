using ErrorOr;
using MediatR;

namespace Application.Teams.Commands.CreateTeam;

public sealed record CreateTeamCommand(
    Guid ProjectId,
    string Name,
    string Description) : IRequest<ErrorOr<Guid>>;