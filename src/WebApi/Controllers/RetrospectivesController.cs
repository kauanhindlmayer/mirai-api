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
    /// Create a new retrospective session.
    /// </summary>
    /// <param name="teamId">The team ID.</param>
    /// <param name="request">The details of the retrospective session to create.</param>
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
            teamId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            retrospectiveId => CreatedAtAction(
                nameof(GetRetrospective),
                new { TeamId = teamId, RetrospectiveId = retrospectiveId },
                retrospectiveId),
            Problem);
    }

    /// <summary>
    /// Get a retrospective session by ID.
    /// </summary>
    /// <param name="retrospectiveId">The retrospective session ID.</param>
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
    /// <param name="teamId">The team ID.</param>
    /// <param name="retrospectiveId">The ID of the retrospective session to create the column in.</param>
    /// <param name="request">The details of the column to create.</param>
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
                new { TeamId = teamId, RetrospectiveId = retrospectiveId },
                retrospectiveId),
            Problem);
    }

    /// <summary>
    /// Create a new item in a column in a retrospective session.
    /// </summary>
    /// <param name="teamId">The team ID.</param>
    /// <param name="retrospectiveId">The ID of the retrospective session to create the item in.</param>
    /// <param name="columnId">The ID of the column to create the item in.</param>
    /// <param name="request">The details of the item to create.</param>
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
            new { TeamId = teamId, RetrospectiveId = retrospectiveId },
            result.Value.Id);
    }

    /// <summary>
    /// Delete an item from a column in a retrospective session.
    /// </summary>
    /// <param name="retrospectiveId">The ID of the retrospective session to delete the item from.</param>
    /// <param name="columnId">The ID of the column to delete the item from.</param>
    /// <param name="itemId">The ID of the item to delete.</param>
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
    /// List all retrospective sessions in a team.
    /// </summary>
    /// <param name="teamId">The team ID.</param>
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