using Application.Retrospectives.Commands.CreateColumn;
using Application.Retrospectives.Commands.CreateItem;
using Application.Retrospectives.Commands.CreateRetrospective;
using Application.Retrospectives.Commands.DeleteItem;
using Application.Retrospectives.Queries.GetRetrospective;
using Application.Retrospectives.Queries.ListRetrospectives;
using Contracts.Retrospectives;
using Domain.Retrospectives;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApi.Hubs;

namespace WebApi.Controllers;

[Route("api/teams/{teamId:guid}/retrospectives")]
public class RetrospectivesController(
    ISender _mediator,
    IHubContext<RetrospectiveHub, IRetrospectiveHub> _hubContext) : ApiController
{
    /// <summary>
    /// Create a new retrospective session.
    /// </summary>
    /// <param name="teamId">The team ID.</param>
    /// <param name="request">The details of the retrospective session to create.</param>
    [HttpPost]
    [ProducesResponseType(typeof(RetrospectiveResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRetrospective(Guid teamId, CreateRetrospectiveRequest request)
    {
        var command = new CreateRetrospectiveCommand(
            request.Title,
            request.Description,
            teamId);

        var result = await _mediator.Send(command);

        return result.Match(
            retrospective => CreatedAtAction(
                actionName: nameof(GetRetrospective),
                routeValues: new { TeamId = teamId, RetrospectiveId = retrospective.Id },
                value: ToDto(retrospective)),
            Problem);
    }

    /// <summary>
    /// Get a retrospective session by ID.
    /// </summary>
    /// <param name="retrospectiveId">The retrospective session ID.</param>
    [HttpGet("{retrospectiveId:guid}")]
    [ProducesResponseType(typeof(RetrospectiveResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRetrospective(Guid retrospectiveId)
    {
        var query = new GetRetrospectiveQuery(retrospectiveId);

        var result = await _mediator.Send(query);

        return result.Match(
            retrospective => Ok(ToDto(retrospective)),
            Problem);
    }

    /// <summary>
    /// Create a new column in a retrospective session.
    /// </summary>
    /// <param name="retrospectiveId">The ID of the retrospective session to create the column in.</param>
    /// <param name="request">The details of the column to create.</param>
    [HttpPost("{retrospectiveId:guid}/columns")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateColumn(Guid retrospectiveId, CreateColumnRequest request)
    {
        var command = new CreateColumnCommand(request.Title, retrospectiveId);

        var result = await _mediator.Send(command);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Create a new item in a column in a retrospective session.
    /// </summary>
    /// <param name="retrospectiveId">The ID of the retrospective session to create the item in.</param>
    /// <param name="columnId">The ID of the column to create the item in.</param>
    /// <param name="request">The details of the item to create.</param>
    [HttpPost("{retrospectiveId:guid}/columns/{columnId:guid}/items")]
    [ProducesResponseType(typeof(RetrospectiveItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateItem(Guid retrospectiveId, Guid columnId, CreateItemRequest request)
    {
        var command = new CreateItemCommand(request.Description, retrospectiveId, columnId);
        var result = await _mediator.Send(command);

        if (result.IsError)
        {
            return Problem(result.Errors);
        }

        var retrospectiveItem = ToDto(result.Value);
        await _hubContext.Clients.All.SendRetrospectiveItem(retrospectiveItem);
        return StatusCode(StatusCodes.Status201Created, retrospectiveItem);
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
    public async Task<IActionResult> DeleteItem(Guid retrospectiveId, Guid columnId, Guid itemId)
    {
        var command = new DeleteItemCommand(retrospectiveId, columnId, itemId);

        var result = await _mediator.Send(command);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// List all retrospective sessions in a team.
    /// </summary>
    /// <param name="teamId">The team ID.</param>
    [HttpGet]
    [ProducesResponseType(typeof(List<RetrospectiveResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListRetrospectives(Guid teamId)
    {
        var query = new ListRetrospectivesQuery(teamId);

        var result = await _mediator.Send(query);

        return result.Match(
            retrospectives => Ok(retrospectives.Select(ToDto)),
            Problem);
    }

    private static RetrospectiveResponse ToDto(Retrospective retrospective)
    {
        return new(
            retrospective.Id,
            retrospective.Title,
            retrospective.Description,
            retrospective.Columns.OrderBy(c => c.Position).Select(ToDto).ToList());
    }

    private static RetrospectiveColumnResponse ToDto(RetrospectiveColumn column)
    {
        return new(
            column.Id,
            column.Title,
            column.Position,
            column.Items.OrderBy(c => c.Position).Select(ToDto).ToList());
    }

    private static RetrospectiveItemResponse ToDto(RetrospectiveItem item)
    {
        return new(
            item.Id,
            item.Description,
            item.Position,
            item.AuthorId,
            item.Votes);
    }
}