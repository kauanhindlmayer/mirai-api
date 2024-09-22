using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Mirai.Api.Hubs;
using Mirai.Application.Retrospectives.Commands.AddColumn;
using Mirai.Application.Retrospectives.Commands.AddItem;
using Mirai.Application.Retrospectives.Commands.CreateRetrospective;
using Mirai.Application.Retrospectives.Queries.GetRetrospective;
using Mirai.Contracts.Retrospectives;
using Mirai.Domain.Retrospectives;

namespace Mirai.Api.Controllers;

public class RetrospectivesController(
    ISender _mediator,
    IHubContext<RetrospectiveHub, IRetrospectiveHub> _hubContext) : ApiController
{
    /// <summary>
    /// Create a new retrospective session.
    /// </summary>
    /// <param name="request">The details of the retrospective session to create.</param>
    [HttpPost(ApiEndpoints.Retrospectives.Create)]
    [ProducesResponseType(typeof(RetrospectiveResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRetrospective(CreateRetrospectiveRequest request)
    {
        var command = new CreateRetrospectiveCommand(
            Title: request.Title,
            Description: request.Description,
            TeamId: request.TeamId);

        var result = await _mediator.Send(command);

        return result.Match(
            retrospective => CreatedAtAction(
                actionName: nameof(GetRetrospective),
                routeValues: new { RetrospectiveId = retrospective.Id },
                value: ToDto(retrospective)),
            Problem);
    }

    /// <summary>
    /// Get a retrospective session by ID.
    /// </summary>
    /// <param name="retrospectiveId">The retrospective session ID.</param>
    [HttpGet(ApiEndpoints.Retrospectives.Get)]
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
    [HttpPost(ApiEndpoints.Retrospectives.AddColumn)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddColumn(Guid retrospectiveId, AddColumnRequest request)
    {
        var command = new AddColumnCommand(request.Title, retrospectiveId);
        var result = await _mediator.Send(command);

        // TODO: Return 201 Created with the new column in the response body.
        return result.Match(_ => NoContent(), Problem);
    }

    /// <summary>
    /// Add an item to a column in a retrospective session.
    /// </summary>
    /// <param name="retrospectiveId">The ID of the retrospective session to add the item to.</param>
    /// <param name="columnId">The ID of the column to add the item to.</param>
    /// <param name="request">The details of the item to add.</param>
    [HttpPost(ApiEndpoints.Retrospectives.AddItem)]
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
        return new RetrospectiveResponse(
            Id: retrospective.Id,
            Name: retrospective.Title,
            Description: retrospective.Description,
            Columns: retrospective.Columns.Select(ToDto).ToList());
    }

    private static RetrospectiveColumnResponse ToDto(RetrospectiveColumn column)
    {
        return new RetrospectiveColumnResponse(
            Id: column.Id,
            Title: column.Title,
            Items: column.Items.Select(ToDto).ToList());
    }

    private static RetrospectiveItemResponse ToDto(RetrospectiveItem item)
    {
        return new RetrospectiveItemResponse(
            Id: item.Id,
            Description: item.Description,
            AuthorId: item.AuthorId,
            Votes: item.Votes);
    }
}