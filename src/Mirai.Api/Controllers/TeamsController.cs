using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.Teams.Commands.AddMember;
using Mirai.Application.Teams.Commands.CreateTeam;
using Mirai.Application.Teams.Queries.GetTeam;
using Mirai.Contracts.Teams;
using Mirai.Domain.Teams;
using Mirai.Domain.Users;

namespace Mirai.Api.Controllers;

public class TeamsController(ISender _mediator) : ApiController
{
    /// <summary>
    /// Create a new team.
    /// </summary>
    /// <param name="request">The details of the team to create.</param>
    [HttpPost(ApiEndpoints.Teams.Create)]
    [ProducesResponseType(typeof(TeamResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTeam(CreateTeamRequest request)
    {
        var command = new CreateTeamCommand(
            ProjectId: request.ProjectId,
            Name: request.Name);

        var result = await _mediator.Send(command);

        return result.Match(
            team => CreatedAtAction(
                actionName: nameof(GetTeam),
                routeValues: new { TeamId = team.Id },
                value: ToDto(team)),
            Problem);
    }

    /// <summary>
    /// Get a team by its ID.
    /// </summary>
    /// <param name="teamId">The ID of the team to get.</param>
    [HttpGet(ApiEndpoints.Teams.Get)]
    [ProducesResponseType(typeof(TeamResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTeam(Guid teamId)
    {
        var query = new GetTeamQuery(teamId);

        var result = await _mediator.Send(query);

        return result.Match(
            team => Ok(ToDto(team)),
            Problem);
    }

    /// <summary>
    /// Add a member to a team.
    /// </summary>
    /// <param name="teamId">The ID of the team to add the member to.</param>
    /// <param name="request">The details of the member to add.</param>
    [HttpPost(ApiEndpoints.Teams.AddMember)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddMember(Guid teamId, AddMemberRequest request)
    {
        var command = new AddMemberCommand(
            TeamId: teamId,
            MemberId: request.MemberId);

        var result = await _mediator.Send(command);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    private static TeamResponse ToDto(Team team)
    {
        return new(
            Id: team.Id,
            ProjectId: team.ProjectId,
            Name: team.Name,
            Members: team.Members.Select(ToDto).ToList(),
            CreatedAt: team.CreatedAt,
            UpdatedAt: team.UpdatedAt);
    }

    private static MemberResponse ToDto(User user)
    {
        return new(
            Id: user.Id,
            Name: user.FullName);
    }
}