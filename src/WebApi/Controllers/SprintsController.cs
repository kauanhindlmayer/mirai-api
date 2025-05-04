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
    /// Creates a new sprint for the specified team.
    /// </summary>
    /// <param name="teamId">The team's unique identifier.</param>
    /// <param name="request">The request containing sprint details.</param>
    /// <returns>The unique identifier of the created sprint.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> CreateSprint(
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
            sprintId => CreatedAtAction(
                nameof(ListSprints),  // TODO: Update to GetSprint when implemented
                new { teamId },
                sprintId),
            Problem);
    }

    /// <summary>
    /// Retrieves all sprints for the specified team.
    /// </summary>
    /// <remarks>
    /// Sprints are returned sorted by end date in descending order.
    /// </remarks>
    /// <param name="teamId">The team's unique identifier.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SprintResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<SprintResponse>>> ListSprints(
        Guid teamId,
        CancellationToken cancellationToken)
    {
        var query = new ListSprintsQuery(teamId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Adds a work item to the specified sprint.
    /// </summary>
    /// <param name="teamId">The team's unique identifier.</param>
    /// <param name="sprintId">The sprint's unique identifier.</param>
    [HttpPost("{sprintId:guid}/work-items")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> AddWorkItemToSprint(
        Guid teamId,
        Guid sprintId,
        AddWorkItemToSprintRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddWorkItemToSprintCommand(
            teamId,
            sprintId,
            request.WorkItemId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }
}