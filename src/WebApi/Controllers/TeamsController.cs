using Application.Teams.Commands.AddMember;
using Application.Teams.Commands.CreateTeam;
using Application.Teams.Queries.GetTeam;
using Application.Teams.Queries.ListTeams;
using Asp.Versioning;
using Contracts.Teams;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/projects/{projectId:guid}/teams")]
public class TeamsController : ApiController
{
    private readonly ISender _sender;

    public TeamsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Create a new team.
    /// </summary>
    /// <param name="projectId">The ID of the project to create the team in.</param>
    /// <param name="request">The details of the team to create.</param>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateTeam(
        Guid projectId,
        CreateTeamRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateTeamCommand(
            projectId,
            request.Name,
            request.Description);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            teamId => CreatedAtAction(
                nameof(GetTeam),
                new { projectId, teamId },
                teamId),
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

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// List all teams in a project.
    /// </summary>
    /// <param name="projectId">The ID of the project to list the teams for.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TeamBriefResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListTeams(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var query = new ListTeamsQuery(projectId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
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

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }
}