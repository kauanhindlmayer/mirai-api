using System.Linq.Expressions;
using Application.Teams.Queries.GetTeam;
using Application.Teams.Queries.ListTeams;
using Domain.Teams;

namespace Application.Teams.Queries;

internal static class TeamQueries
{
    public static Expression<Func<Team, TeamResponse>> ProjectToDto()
    {
        return t => new TeamResponse
        {
            Id = t.Id,
            ProjectId = t.ProjectId,
            Name = t.Name,
            Members = t.Users.Select(m => new MemberResponse
            {
                Id = m.Id,
                Name = m.FullName,
            }),
            CreatedAtUtc = t.CreatedAtUtc,
            UpdatedAtUtc = t.UpdatedAtUtc,
        };
    }

    public static Expression<Func<Team, TeamBriefResponse>> ProjectToBriefDto()
    {
        return t => new TeamBriefResponse
        {
            Id = t.Id,
            Name = t.Name,
            BoardId = t.Board.Id,
        };
    }
}