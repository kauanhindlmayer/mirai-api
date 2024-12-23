using Application.Teams.Commands.AddMember;
using Application.Teams.Commands.CreateTeam;
using Application.Teams.Queries.GetTeam;
using Asp.Versioning;
using Contracts.Teams;
using Domain.Teams;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/projects/{projectId:guid}/teams")]
public class TeamsController(ISender sender) : ApiController
{
    /// <summary>
    /// Create a new team.
    /// </summary>
    /// <param name="projectId">The ID of the project to create the team in.</param>
    /// <param name="request">The details of the team to create.</param>
    [HttpPost]
    [ProducesResponseType(typeof(TeamResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateTeam(
        Guid projectId,
        CreateTeamRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateTeamCommand(projectId, request.Name);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            team => CreatedAtAction(
                actionName: nameof(GetTeam),
                routeValues: new { ProjectId = projectId, TeamId = team.Id },
                value: ToDto(team)),
            Problem);
    }

    /// <summary>
    /// Get a team by its ID.
    /// </summary>
    /// <param name="teamId">The ID of the team to get.</param>
    [HttpGet("{teamId:guid}")]
    [ProducesResponseType(typeof(TeamResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTeam(
        Guid teamId,
        CancellationToken cancellationToken)
    {
        var query = new GetTeamQuery(teamId);

        var result = await sender.Send(query, cancellationToken);

        return result.Match(
            team => Ok(ToDto(team)),
            Problem);
    }

    /// <summary>
    /// Add a member to a team.
    /// </summary>
    /// <param name="teamId">The ID of the team to add the member to.</param>
    /// <param name="request">The details of the member to add.</param>
    [HttpPost("{teamId:guid}/members")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddMember(
        Guid teamId,
        AddMemberRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddMemberCommand(teamId, request.MemberId);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    private static TeamResponse ToDto(Team team)
    {
        return new(
            team.Id,
            team.ProjectId,
            team.Name,
            team.Members.Select(ToDto).ToList(),
            team.CreatedAt,
            team.UpdatedAt);
    }

    private static MemberResponse ToDto(User user) => new(user.Id, user.FullName);
}