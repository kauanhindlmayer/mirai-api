using Application.Teams.Queries.GetTeam;
using ErrorOr;
using MediatR;

namespace Application.Teams.Queries.ListTeams;

public sealed record ListTeamsQuery(Guid ProjectId) : IRequest<ErrorOr<TeamBriefResponse>>;