using Application.Retrospectives.Commands.AddColumn;
using Application.Retrospectives.Commands.AddItem;
using Application.Retrospectives.Commands.CreateRetrospective;
using Application.Retrospectives.Queries.GetRetrospective;
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
    /// Add a column to a retrospective session.
    /// </summary>
    /// <param name="retrospectiveId">The ID of the retrospective session to add the column to.</param>
    /// <param name="request">The details of the column to add.</param>
    [HttpPost("{retrospectiveId:guid}/columns")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddColumn(Guid retrospectiveId, AddColumnRequest request)
    {
        var command = new AddColumnCommand(request.Title, retrospectiveId);

        var result = await _mediator.Send(command);

        // TODO: Return 201 Created with the new column in the response body.
        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Add an item to a column in a retrospective session.
    /// </summary>
    /// <param name="retrospectiveId">The ID of the retrospective session to add the item to.</param>
    /// <param name="columnId">The ID of the column to add the item to.</param>
    /// <param name="request">The details of the item to add.</param>
    [HttpPost("{retrospectiveId:guid}/columns/{columnId:guid}/items")]
    [ProducesResponseType(typeof(RetrospectiveItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddItem(Guid retrospectiveId, Guid columnId, AddItemRequest request)
    {
        var command = new AddItemCommand(request.Description, retrospectiveId, columnId);
        var result = await _mediator.Send(command);

        if (result.IsError)
        {
            return Problem(result.Errors);
        }

        var retrospectiveItem = ToDto(result.Value);
        await _hubContext.Clients.All.SendRetrospectiveItem(retrospectiveItem);
        return StatusCode(StatusCodes.Status201Created, retrospectiveItem);
    }

    private static RetrospectiveResponse ToDto(Retrospective retrospective)
    {
        return new(
            retrospective.Id,
            retrospective.Title,
            retrospective.Description,
            retrospective.Columns.Select(ToDto).ToList());
    }

    private static RetrospectiveColumnResponse ToDto(RetrospectiveColumn column)
    {
        return new(
            column.Id,
            column.Title,
            column.Items.Select(ToDto).ToList());
    }

    private static RetrospectiveItemResponse ToDto(RetrospectiveItem item)
    {
        return new(
            item.Id,
            item.Description,
            item.AuthorId,
            item.Votes);
    }
}