using Domain.Teams;
using ErrorOr;
using MediatR;

namespace Application.Teams.Queries.GetTeam;

public record GetTeamQuery(Guid TeamId) : IRequest<ErrorOr<Team>>;