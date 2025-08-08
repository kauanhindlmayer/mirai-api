using System.Net.Mime;
using Application.Projects.Commands.AddUserToProject;
using Application.Projects.Commands.CreateProject;
using Application.Projects.Commands.RemoveUserFromProject;
using Application.Projects.Commands.UpdateProject;
using Application.Projects.Queries.GetProject;
using Application.Projects.Queries.ListProjects;
using Asp.Versioning;
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
}