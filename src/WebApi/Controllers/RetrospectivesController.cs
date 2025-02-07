using Application.Retrospectives.Commands.CreateColumn;
using Application.Retrospectives.Commands.CreateItem;
using Application.Retrospectives.Commands.CreateRetrospective;
using Application.Retrospectives.Commands.DeleteItem;
using Application.Retrospectives.Queries.GetRetrospective;
using Application.Retrospectives.Queries.ListRetrospectives;
using Asp.Versioning;
using Contracts.Retrospectives;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApi.Hubs;

namespace WebApi.Controllers;

[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/teams/{teamId:guid}/retrospectives")]
public class RetrospectivesController : ApiController
{
    private readonly ISender _sender;
    private readonly IHubContext<RetrospectiveHub, IRetrospectiveHub> _hubContext;

    public RetrospectivesController(
        ISender sender,
        IHubContext<RetrospectiveHub, IRetrospectiveHub> hubContext)
    {
        _sender = sender;
        _hubContext = hubContext;
    }

    /// <summary>
    /// Create a retrospective session.
    /// </summary>
    /// <remarks>
    /// Creates a new retrospective session for the specified team. When a retrospective
    /// session is created, a default set of columns are automatically set up.
    /// </remarks>
    /// <param name="teamId">The team's unique identifier.</param>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRetrospective(
        Guid teamId,
        CreateRetrospectiveRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateRetrospectiveCommand(
            request.Title,
            request.Description,
            request.Template,
            teamId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            retrospectiveId => CreatedAtAction(
                nameof(GetRetrospective),
                new { teamId, retrospectiveId },
                retrospectiveId),
            Problem);
    }

    /// <summary>
    /// Retrieve a retrospective session.
    /// </summary>
    /// <remarks>
    /// Retrieves the retrospective session with the specified unique identifier.
    /// </remarks>
    /// <param name="retrospectiveId">The retrospective session's unique identifier.</param>
    [HttpGet("{retrospectiveId:guid}")]
    [ProducesResponseType(typeof(RetrospectiveResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRetrospective(
        Guid retrospectiveId,
        CancellationToken cancellationToken)
    {
        var query = new GetRetrospectiveQuery(retrospectiveId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Create a new column in a retrospective session.
    /// </summary>
    /// <remarks>
    /// Creates a new column in the specified retrospective session.
    /// </remarks>
    /// <param name="teamId">The team's unique identifier.</param>
    /// <param name="retrospectiveId">The retrospective session's unique identifier.</param>
    [HttpPost("{retrospectiveId:guid}/columns")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateColumn(
        Guid teamId,
        Guid retrospectiveId,
        CreateColumnRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateColumnCommand(request.Title, retrospectiveId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => CreatedAtAction(
                nameof(GetRetrospective),
                new { teamId, retrospectiveId },
                retrospectiveId),
            Problem);
    }

    /// <summary>
    /// Create a new retrospective item.
    /// </summary>
    /// <remarks>
    /// Creates a new item in the specified column of the retrospective session.
    /// </remarks>
    /// <param name="teamId">The team's unique identifier.</param>
    /// <param name="retrospectiveId">The retrospective session's unique identifier.</param>
    /// <param name="columnId">The column's unique identifier.</param>
    [HttpPost("{retrospectiveId:guid}/columns/{columnId:guid}/items")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateItem(
        Guid teamId,
        Guid retrospectiveId,
        Guid columnId,
        CreateItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateItemCommand(
            request.Description,
            retrospectiveId,
            columnId);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsError)
        {
            return Problem(result.Errors);
        }

        await _hubContext.Clients.All.SendRetrospectiveItem(result.Value);
        return CreatedAtAction(
            nameof(GetRetrospective),
            new { teamId, retrospectiveId },
            result.Value.Id);
    }

    /// <summary>
    /// Delete a retrospective item.
    /// </summary>
    /// <remarks>
    /// Deletes the item with the specified unique identifier from the retrospective session.
    /// </remarks>
    /// <param name="retrospectiveId">The retrospective session's unique identifier.</param>
    /// <param name="columnId">The column's unique identifier.</param>
    /// <param name="itemId">The item's unique identifier.</param>
    [HttpDelete("{retrospectiveId:guid}/columns/{columnId:guid}/items/{itemId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteItem(
        Guid retrospectiveId,
        Guid columnId,
        Guid itemId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteItemCommand(retrospectiveId, columnId, itemId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Retrieve all retrospective sessions for a team.
    /// </summary>
    /// <remarks>
    /// Returns a list of retrospective sessions for the specified team.
    /// </remarks>
    /// <param name="teamId">The team's unique identifier.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<RetrospectiveResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListRetrospectives(
        Guid teamId,
        CancellationToken cancellationToken)
    {
        var query = new ListRetrospectivesQuery(teamId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }
}