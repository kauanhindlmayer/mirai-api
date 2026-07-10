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
            Description = t.Description,
            IsDefault = t.IsDefault,
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
            Description = t.Description,
            BoardId = t.Board.Id,
            IsDefault = t.IsDefault,
            MemberCount = t.Members.Count,
        };
    }
}