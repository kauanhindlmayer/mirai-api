using Application.Sprints.Commands.AddWorkItemToSprint;
using Application.Sprints.Commands.CreateSprint;
using Application.Sprints.Queries.ListSprints;
using Asp.Versioning;
using Contracts.Sprints;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/teams/{teamId:guid}/sprints")]
public sealed class SprintsController : ApiController
{
    private readonly ISender _sender;

    public SprintsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Create a sprint.
    /// </summary>
    /// <remarks>
    /// Creates a new sprint object.
    /// </remarks>
    /// <param name="teamId">The team's unique identifier.</param>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateSprint(
        Guid teamId,
        CreateSprintRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateSprintCommand(
            teamId,
            request.Name,
            request.StartDate,
            request.EndDate);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            sprintId => Ok(sprintId),
            Problem);
    }

    /// <summary>
    /// Retrieve all sprints for a team.
    /// </summary>
    /// <remarks>
    /// Returns a list of sprints. The sprints are returned sorted by their end
    /// date in descending order, with the most recent sprint appearing first.
    /// </remarks>
    /// <param name="teamId">The team's unique identifier.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SprintResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListSprints(
        Guid teamId,
        CancellationToken cancellationToken)
    {
        var query = new ListSprintsQuery(teamId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Add a work item to a sprint.
    /// </summary>
    /// <remarks>
    /// Adds a work item to the specified sprint.
    /// </remarks>
    /// <param name="teamId">The team's unique identifier.</param>
    /// <param name="sprintId">The sprint's unique identifier.</param>
    [HttpPost("{sprintId:guid}/work-items")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddWorkItemToSprint(
        Guid teamId,
        Guid sprintId,
        AddWorkItemToSprintRequest request,
        CancellationToken cancellationToken)
    {
        var query = new AddWorkItemToSprintCommand(
            teamId,
            sprintId,
            request.WorkItemId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }
}