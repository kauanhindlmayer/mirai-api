using ErrorOr;
using MediatR;
using Mirai.Domain.Teams;

namespace Mirai.Application.Teams.Queries.GetTeam;

public record GetTeamQuery(Guid TeamId) : IRequest<ErrorOr<Team>>;