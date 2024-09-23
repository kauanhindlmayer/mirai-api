using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.Projects.Commands.CreateProject;
using Mirai.Application.Projects.Commands.CreateTagCommand;
using Mirai.Application.Projects.Queries.GetProject;
using Mirai.Application.Projects.Queries.ListProjects;
using Mirai.Contracts.Projects;
using Mirai.Contracts.Tags;
using Mirai.Domain.Projects;

namespace Mirai.Api.Controllers;

public class ProjectsController(ISender _mediator) : ApiController
{
    /// <summary>
    /// Create a new project.
    /// </summary>
    /// <param name="request">The project data.</param>
    [HttpPost(ApiEndpoints.Projects.Create)]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProject(CreateProjectRequest request)
    {
        var command = new CreateProjectCommand(
            request.Name,
            request.Description,
            request.OrganizationId);

        var result = await _mediator.Send(command);

        return result.Match(
            project => CreatedAtAction(
                actionName: nameof(GetProject),
                routeValues: new { ProjectId = project.Id },
                value: ToDto(project)),
            Problem);
    }

    /// <summary>
    /// Get a project by ID.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    [HttpGet(ApiEndpoints.Projects.Get)]
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
    [HttpGet(ApiEndpoints.Organizations.ListProjects)]
    [ProducesResponseType(typeof(IEnumerable<ProjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListProjects(Guid organizationId)
    {
        var query = new ListProjectsQuery(organizationId);

        var result = await _mediator.Send(query);

        return result.Match(
            projects => Ok(projects.ConvertAll(ToDto)),
            Problem);
    }

    /// <summary>
    /// Add a tag to a project that can be used to categorize work items.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="request">The tag data.</param>
    [HttpPost(ApiEndpoints.Projects.AddTag)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddTag(Guid projectId, CreateTagRequest request)
    {
        var command = new CreateTagCommand(projectId, request.Name);

        var result = await _mediator.Send(command);

        return result.Match(
            _ => NoContent(),
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