using Domain.Teams;
using ErrorOr;
using MediatR;

namespace Application.Teams.Commands.CreateTeam;

public record CreateTeamCommand(Guid ProjectId, string Name)
    : IRequest<ErrorOr<Team>>;