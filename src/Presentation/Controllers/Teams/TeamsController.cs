using System.Net.Mime;
using Application.Abstractions;
using Application.Authorization.Queries.GetEffectivePermissions;
using Application.Teams.Commands.AddUserToTeam;
using Application.Teams.Commands.ChangeTeamMemberRole;
using Application.Teams.Commands.CreateTeam;
using Application.Teams.Commands.RemoveUserFromTeam;
using Application.Teams.Queries.GetTeam;
using Application.Teams.Queries.GetTeamMembers;
using Application.Teams.Queries.ListTeams;
using Asp.Versioning;
using Domain.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Constants;

namespace Presentation.Controllers.Teams;

[ApiVersion(ApiVersions.V1)]
[Route("api/projects/{projectId:guid}/teams")]
[Produces(MediaTypeNames.Application.Json, CustomMediaTypeNames.Application.JsonV1)]
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
    public async Task<ActionResult> AddUserToTeam(
        Guid teamId,
        AddUserToTeamRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddUserToTeamCommand(teamId, request.UserId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Retrieve the members of a team.
    /// </summary>
    /// <param name="teamId">The team's unique identifier.</param>
    /// <param name="pageRequest">Pagination parameters.</param>
    [HttpGet("{teamId:guid}/members")]
    [ProducesResponseType(typeof(PaginatedList<TeamMemberResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaginatedList<TeamMemberResponse>>> GetTeamMembers(
        Guid teamId,
        [FromQuery] PageRequest pageRequest,
        CancellationToken cancellationToken)
    {
        var query = new GetTeamMembersQuery(
            teamId,
            pageRequest.Page,
            pageRequest.PageSize);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Remove a member from the specified team.
    /// </summary>
    /// <param name="teamId">The team's unique identifier.</param>
    /// <param name="userId">The user's unique identifier.</param>
    [HttpDelete("{teamId:guid}/members/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> RemoveUserFromTeam(
        Guid teamId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveUserFromTeamCommand(teamId, userId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Change a team member's role.
    /// </summary>
    [HttpPut("{teamId:guid}/members/{userId:guid}/role")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> ChangeTeamMemberRole(
        Guid teamId,
        Guid userId,
        ChangeTeamMemberRoleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ChangeTeamMemberRoleCommand(
            teamId,
            userId,
            request.RoleId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Retrieve the caller's effective permissions within a team.
    /// </summary>
    [HttpGet("/api/teams/{teamId:guid}/effective-permissions")]
    [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<string>>> GetEffectivePermissions(
        Guid teamId,
        CancellationToken cancellationToken)
    {
        var query = new GetEffectivePermissionsQuery(ResourceType.Team, teamId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }
}