using System.Net.Mime;
using Application.Abstractions;
using Application.Authorization.Queries.GetEffectivePermissions;
using Application.Projects.Commands.AddUserToProject;
using Application.Projects.Commands.ChangeProjectMemberRole;
using Application.Projects.Commands.CreateProject;
using Application.Projects.Commands.RemoveUserFromProject;
using Application.Projects.Commands.UpdateProject;
using Application.Projects.Queries.GetMentionableProjectUsers;
using Application.Projects.Queries.GetProject;
using Application.Projects.Queries.GetProjectUsers;
using Application.Projects.Queries.ListProjects;
using Application.Projects.Queries.ResolveProjectUsers;
using Asp.Versioning;
using Domain.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Constants;

namespace Presentation.Controllers.Projects;

[ApiVersion(ApiVersions.V1)]
[Route("api/organizations/{organizationId:guid}/projects")]
[Produces(MediaTypeNames.Application.Json, CustomMediaTypeNames.Application.JsonV1)]
public sealed class ProjectsController : ApiController
{
    private readonly ISender _sender;

    public ProjectsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Create a project.
    /// </summary>
    /// <remarks>
    /// When a project is created, a default team, board, wiki page, and an
    /// initial 14-day sprint are automatically set up.
    /// </remarks>
    /// <param name="organizationId">The organization's unique identifier.</param>
    /// <returns>The unique identifier of the created project.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateProject(
        Guid organizationId,
        CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateProjectCommand(
            request.Name,
            request.Description,
            organizationId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            projectId => CreatedAtAction(
                nameof(GetProject),
                new { projectId },
                projectId),
            Problem);
    }

    /// <summary>
    /// Retrieve a project.
    /// </summary>
    /// <param name="projectId">The project's unique identifier.</param>
    [HttpGet("/api/projects/{projectId:guid}")]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectResponse>> GetProject(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var query = new GetProjectQuery(projectId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Retrieve all projects for an organization.
    /// </summary>
    /// <param name="organizationId">The organization's unique identifier.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProjectResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProjectResponse>>> ListProjects(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var query = new ListProjectsQuery(organizationId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Update a project.
    /// </summary>
    /// <param name="organizationId">The organization's unique identifier.</param>
    /// <param name="projectId">The project's unique identifier.</param>
    /// <returns>The unique identifier of the updated project.</returns>
    [HttpPut("{projectId:guid}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> UpdateProject(
        Guid organizationId,
        Guid projectId,
        UpdateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProjectCommand(
            organizationId,
            projectId,
            request.Name,
            request.Description);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => Ok(projectId),
            Problem);
    }

    /// <summary>
    /// Add a user to a project.
    /// </summary>
    /// <param name="projectId">The project's unique identifier.</param>
    [HttpPost("{projectId:guid}/users")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> AddUserToProject(
        Guid projectId,
        AddUserToProjectRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddUserToProjectCommand(
            projectId,
            request.UserId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Remove a user from a project.
    /// </summary>
    /// <param name="projectId">The project's unique identifier.</param>
    /// <param name="userId">The user's unique identifier.</param>
    [HttpDelete("{projectId:guid}/users/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> RemoveUserFromProject(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveUserFromProjectCommand(
            projectId,
            userId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Get users that belong to a project.
    /// </summary>
    /// <param name="projectId">The project's unique identifier.</param>
    /// <param name="pageRequest">Pagination and sorting parameters.</param>
    [HttpGet("{projectId:guid}/users")]
    [ProducesResponseType(typeof(PaginatedList<ProjectUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaginatedList<ProjectUserResponse>>> GetProjectUsers(
        Guid projectId,
        [FromQuery] PageRequest pageRequest,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProjectUsersQuery(
            projectId,
            pageRequest.Page,
            pageRequest.PageSize,
            pageRequest.Sort,
            pageRequest.SearchTerm);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Get users eligible to be @mentioned within a project.
    /// </summary>
    /// <remarks>
    /// Unlike <see cref="GetProjectUsers"/>, which lists only direct project
    /// members for membership management, this includes anyone with
    /// effective access to the project - such as organization owners and
    /// admins - even without a direct membership record.
    /// </remarks>
    /// <param name="projectId">The project's unique identifier.</param>
    /// <param name="pageRequest">Pagination parameters and the search term.</param>
    [HttpGet("{projectId:guid}/users/mentionable")]
    [ProducesResponseType(typeof(PaginatedList<MentionableProjectUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaginatedList<MentionableProjectUserResponse>>> GetMentionableProjectUsers(
        Guid projectId,
        [FromQuery] PageRequest pageRequest,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMentionableProjectUsersQuery(
            projectId,
            pageRequest.Page,
            pageRequest.PageSize,
            pageRequest.SearchTerm);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Resolve users by id within a project's organization, regardless of
    /// their current project membership.
    /// </summary>
    /// <remarks>
    /// Used to resolve users who are no longer project members (e.g. an
    /// @mention referencing someone since removed from the project). A
    /// user id with no match (not in the same organization, or never
    /// existed) is simply omitted from the response.
    /// </remarks>
    /// <param name="projectId">The project's unique identifier.</param>
    /// <param name="request">The user ids to resolve.</param>
    [HttpGet("{projectId:guid}/users/resolve")]
    [ProducesResponseType(typeof(IReadOnlyList<ResolvedUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ResolvedUserResponse>>> ResolveProjectUsers(
        Guid projectId,
        [FromQuery] ResolveProjectUsersRequest request,
        CancellationToken cancellationToken)
    {
        var query = new ResolveProjectUsersQuery(projectId, request.UserIds);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Change a project member's role.
    /// </summary>
    [HttpPut("{projectId:guid}/users/{userId:guid}/role")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> ChangeProjectMemberRole(
        Guid projectId,
        Guid userId,
        ChangeProjectMemberRoleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ChangeProjectMemberRoleCommand(
            projectId,
            userId,
            request.RoleId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Retrieve the caller's effective permissions within a project.
    /// </summary>
    [HttpGet("/api/projects/{projectId:guid}/effective-permissions")]
    [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<string>>> GetEffectivePermissions(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var query = new GetEffectivePermissionsQuery(ResourceType.Project, projectId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }
}