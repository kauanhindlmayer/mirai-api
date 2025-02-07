using Application.Projects.Commands.CreateProject;
using Application.Projects.Commands.UpdateProject;
using Application.Projects.Queries.GetProject;
using Application.Projects.Queries.ListProjects;
using Asp.Versioning;
using Contracts.Projects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/organizations/{organizationId:guid}/projects")]
public class ProjectsController : ApiController
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
    /// Creates a new project within an organization. When a project is created,
    /// a default team, board, wiki page and an initial 14-day sprint are
    /// automatically set up.
    /// </remarks>
    /// <param name="organizationId">The organization's unique identifier.</param>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProject(
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
    /// <remarks>
    /// Retrieves the project with the specified unique identifier.
    /// </remarks>
    /// <param name="projectId">The project's unique identifier.</param>
    [HttpGet("/api/v{version:apiVersion}/projects/{projectId:guid}")]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProject(
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
    /// <remarks>
    /// Returns a list of projects for the specified organization.
    /// </remarks>
    /// <param name="organizationId">The organization's unique identifier.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListProjects(
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
    /// <remarks>
    /// Updates the project with the specified unique identifier by setting the
    /// values of the parameters passed. Any parameters not provided will be
    /// left unchanged.
    /// </remarks>
    /// <param name="organizationId">The organization's unique identifier.</param>
    /// <param name="projectId">The project's unique identifier.</param>
    [HttpPut("{projectId:guid}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProject(
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
}