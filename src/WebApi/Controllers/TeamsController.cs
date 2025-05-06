using Application.Teams.Commands.AddMember;
using Application.Teams.Commands.CreateTeam;
using Application.Teams.Queries.GetTeam;
using Application.Teams.Queries.ListTeams;
using Asp.Versioning;
using Contracts.Teams;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Constants;

namespace WebApi.Controllers;

[ApiVersion(ApiVersions.V1)]
[Route("api/projects/{projectId:guid}/teams")]
public sealed class TeamsController : ApiController
{
    private readonly ISender _sender;

    public TeamsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Create a team.
    /// </summary>
    /// <param name="projectId">The project's unique identifier.</param>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> CreateTeam(
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
    /// Retrieve a team.
    /// </summary>
    /// <param name="teamId">The team's unique identifier.</param>
    [HttpGet("{teamId:guid}")]
    [ProducesResponseType(typeof(TeamResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeamResponse>> GetTeam(
        Guid teamId,
        CancellationToken cancellationToken)
    {
        var query = new GetTeamQuery(teamId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Retrieve all teams for a project.
    /// </summary>
    /// <param name="projectId">The project's unique identifier.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TeamBriefResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<TeamBriefResponse>>> ListTeams(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var query = new ListTeamsQuery(projectId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Add a member to the specified team.
    /// </summary>
    /// <param name="teamId">The team's unique identifier.</param>
    [HttpPost("{teamId:guid}/members")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> AddMember(
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