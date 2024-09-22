using ErrorOr;
using MediatR;
using Mirai.Domain.Teams;

namespace Mirai.Application.Teams.Commands.CreateTeam;

public record CreateTeamCommand(Guid ProjectId, string Name)
    : IRequest<ErrorOr<Team>>;