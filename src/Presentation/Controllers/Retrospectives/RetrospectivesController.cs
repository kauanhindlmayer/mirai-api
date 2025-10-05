using System.Net.Mime;
using Application.Retrospectives.Commands.CreateRetrospective;
using Application.Retrospectives.Commands.CreateRetrospectiveColumn;
using Application.Retrospectives.Commands.CreateRetrospectiveItem;
using Application.Retrospectives.Commands.DeleteRetrospective;
using Application.Retrospectives.Commands.DeleteRetrospectiveItem;
using Application.Retrospectives.Commands.UpdateRetrospective;
using Application.Retrospectives.Queries.GetRetrospective;
using Application.Retrospectives.Queries.ListRetrospectives;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Presentation.Constants;
using Presentation.Hubs;

namespace Presentation.Controllers.Retrospectives;

[ApiVersion(ApiVersions.V1)]
[Route("api/retrospectives")]
[Produces(MediaTypeNames.Application.Json, CustomMediaTypeNames.Application.JsonV1)]
public sealed class RetrospectivesController : ApiController
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
    /// When a retrospective is created, default columns are initialized based
    /// on the chosen template.
    /// </remarks>
    /// <returns>The unique identifier of the created retrospective.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateRetrospective(
        CreateRetrospectiveRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateRetrospectiveCommand(
            request.Title,
            request.MaxVotesPerUser,
            request.Template,
            request.TeamId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            retrospectiveId => CreatedAtAction(
                nameof(GetRetrospective),
                new { retrospectiveId },
                retrospectiveId),
            Problem);
    }

    /// <summary>
    /// Retrieve a retrospective session.
    /// </summary>
    /// <param name="retrospectiveId">The retrospective session's unique identifier.</param>
    [HttpGet("{retrospectiveId:guid}")]
    [ProducesResponseType(typeof(RetrospectiveResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RetrospectiveResponse>> GetRetrospective(
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
    /// <param name="retrospectiveId">The retrospective session's unique identifier.</param>
    /// <returns>The unique identifier of retrospective session.</returns>
    [HttpPost("{retrospectiveId:guid}/columns")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> CreateColumn(
        Guid retrospectiveId,
        CreateColumnRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateRetrospectiveColumnCommand(
            request.Title,
            retrospectiveId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            columnId => CreatedAtAction(
                nameof(GetRetrospective),
                new { retrospectiveId },
                columnId),
            Problem);
    }

    /// <summary>
    /// Create a new retrospective item.
    /// </summary>
    /// <param name="retrospectiveId">The retrospective session's unique identifier.</param>
    /// <param name="columnId">The column's unique identifier.</param>
    /// <returns>The unique identifier of the created item.</returns>
    [HttpPost("{retrospectiveId:guid}/columns/{columnId:guid}/items")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> CreateItem(
        Guid retrospectiveId,
        Guid columnId,
        CreateItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateRetrospectiveItemCommand(
            request.Content,
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
            new { retrospectiveId },
            result.Value.Id);
    }

    /// <summary>
    /// Delete a retrospective item.
    /// </summary>
    /// <param name="retrospectiveId">The retrospective session's unique identifier.</param>
    /// <param name="columnId">The column's unique identifier.</param>
    /// <param name="itemId">The item's unique identifier.</param>
    [HttpDelete("{retrospectiveId:guid}/columns/{columnId:guid}/items/{itemId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteItem(
        Guid retrospectiveId,
        Guid columnId,
        Guid itemId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteRetrospectiveItemCommand(
            retrospectiveId,
            columnId,
            itemId);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsError)
        {
            return Problem(result.Errors);
        }

        await _hubContext.Clients.All.DeleteRetrospectiveItem(itemId);
        return NoContent();
    }

    /// <summary>
    /// Retrieve all retrospective sessions for a team.
    /// </summary>
    /// <param name="teamId">The team's unique identifier.</param>
    [HttpGet("/api/teams/{teamId:guid}/retrospectives")]
    [ProducesResponseType(typeof(IReadOnlyList<RetrospectiveResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<RetrospectiveResponse>>> ListRetrospectives(
        Guid teamId,
        CancellationToken cancellationToken)
    {
        var query = new ListRetrospectivesQuery(teamId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Update a retrospective session.
    /// </summary>
    /// <param name="retrospectiveId">The retrospective session's unique identifier.</param>
    /// <param name="request">The request containing the updated details.</param>
    /// <remarks>
    /// Existing feedback items may not be available after changing the board template.
    /// </remarks>
    [HttpPut("{retrospectiveId:guid}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> UpdateRetrospective(
        Guid retrospectiveId,
        UpdateRetrospectiveRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateRetrospectiveCommand(
            retrospectiveId,
            request.Title,
            request.MaxVotesPerUser,
            request.Template);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => CreatedAtAction(
                nameof(GetRetrospective),
                new { retrospectiveId },
                retrospectiveId),
            Problem);
    }

    /// <summary>
    /// Delete a retrospective session.
    /// </summary>
    /// <param name="retrospectiveId">The retrospective session's unique identifier.</param>
    [HttpDelete("{retrospectiveId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteRetrospective(
        Guid retrospectiveId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteRetrospectiveCommand(retrospectiveId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }
}