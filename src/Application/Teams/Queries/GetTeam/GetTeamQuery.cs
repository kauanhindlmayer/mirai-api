using Domain.Teams;
using ErrorOr;
using MediatR;

namespace Application.Teams.Queries.GetTeam;

public sealed record GetTeamQuery(Guid TeamId) : IRequest<ErrorOr<TeamResponse>>;