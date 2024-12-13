using Application.Projects.Commands.CreateProject;
using Application.Projects.Commands.UpdateProject;
using Application.Projects.Queries.GetProject;
using Application.Projects.Queries.ListProjects;
using Asp.Versioning;
using Contracts.Projects;
using Domain.Projects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/organizations/{organizationId:guid}/projects")]
public class ProjectsController(ISender sender) : ApiController
{
    /// <summary>
    /// Create a new project.
    /// </summary>
    /// <param name="organizationId">The ID of the organization to create the project for.</param>
    /// <param name="request">The project data.</param>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status201Created)]
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

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            project => CreatedAtAction(
                actionName: nameof(GetProject),
                routeValues: new { OrganizationId = organizationId, ProjectId = project.Id },
                value: ToDto(project)),
            Problem);
    }

    /// <summary>
    /// Get a project by ID.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    [HttpGet("{projectId:guid}")]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProject(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var query = new GetProjectQuery(projectId);

        var result = await sender.Send(query, cancellationToken);

        return result.Match(
            project => Ok(ToDto(project)),
            Problem);
    }

    /// <summary>
    /// List all projects in an organization.
    /// </summary>
    /// <param name="organizationId">The organization ID.</param>
    [HttpGet]
    [ProducesResponseType(typeof(List<ProjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListProjects(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var query = new ListProjectsQuery(organizationId);

        var result = await sender.Send(query, cancellationToken);

        return result.Match(
            projects => Ok(projects.ConvertAll(ToDto)),
            Problem);
    }

    /// <summary>
    /// Update a project.
    /// </summary>
    /// <param name="organizationId">The organization ID.</param>
    /// <param name="projectId">The project ID.</param>
    /// <param name="request">The project data.</param>
    [HttpPut("{projectId:guid}")]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
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

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            project => Ok(ToDto(project)),
            Problem);
    }

    private static ProjectResponse ToDto(Project project)
    {
        return new(
            project.Id,
            project.Name,
            project.Description,
            project.OrganizationId,
            project.CreatedAt,
            project.UpdatedAt);
    }
}