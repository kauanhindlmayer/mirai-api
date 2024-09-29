using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.Projects.Commands.CreateProject;
using Mirai.Application.Projects.Queries.GetProject;
using Mirai.Application.Projects.Queries.ListProjects;
using Mirai.Contracts.Projects;
using Mirai.Domain.Projects;

namespace Mirai.Api.Controllers;

[Route("api/organizations/{organizationId:guid}/projects")]
public class ProjectsController(ISender _mediator) : ApiController
{
    /// <summary>
    /// Create a new project.
    /// </summary>
    /// <param name="organizationId">The ID of the organization to create the project for.</param>
    /// <param name="request">The project data.</param>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProject(Guid organizationId, CreateProjectRequest request)
    {
        var command = new CreateProjectCommand(
            request.Name,
            request.Description,
            organizationId);

        var result = await _mediator.Send(command);

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
    public async Task<IActionResult> GetProject(Guid projectId)
    {
        var query = new GetProjectQuery(projectId);

        var result = await _mediator.Send(query);

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
    public async Task<IActionResult> ListProjects(Guid organizationId)
    {
        var query = new ListProjectsQuery(organizationId);

        var result = await _mediator.Send(query);

        return result.Match(
            projects => Ok(projects.ConvertAll(ToDto)),
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