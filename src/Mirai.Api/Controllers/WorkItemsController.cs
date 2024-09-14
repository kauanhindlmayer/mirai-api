using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.WorkItems.Commands.CreateWorkItem;
using Mirai.Contracts.WorkItems;
using Mirai.Domain.WorkItems;

namespace Mirai.Api.Controllers;

[AllowAnonymous]
public class WorkItemsController(ISender _mediator) : ApiController
{
    /// <summary>
    /// Create a new work item.
    /// </summary>
    /// <param name="request">The details of the work item to create.</param>
    [HttpPost(ApiEndpoints.WorkItems.Create)]
    [ProducesResponseType(typeof(WorkItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateWorkItem(CreateWorkItemRequest request)
    {
        var command = new CreateWorkItemCommand(
            ProjectId: request.ProjectId,
            Type: request.Type,
            Title: request.Title);

        var result = await _mediator.Send(command);

        return result.Match(
            workItem => CreatedAtAction(
                actionName: nameof(GetWorkItem),
                routeValues: new { WorkItemId = workItem.Id },
                value: ToDto(workItem)),
            Problem);
    }

    /// <summary>
    /// Get a work item by its ID.
    /// </summary>
    /// <param name="workItemId">The ID of the work item to get.</param>
    [HttpGet(ApiEndpoints.WorkItems.Get)]
    [ProducesResponseType(typeof(WorkItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetWorkItem(Guid workItemId)
    {
        // var query = new GetWorkItemQuery(workItemId);

        // var result = await _mediator.Send(query);

        // return result.Match(
        //     workItem => Ok(ToDto(workItem)),
        //     Problem);
        return Ok();
    }

    private static WorkItemResponse ToDto(WorkItem workItem)
    {
        return new(
            workItem.Id,
            workItem.ProjectId,
            workItem.Title,
            workItem.Description,
            workItem.Status.ToString(),
            workItem.Type.ToString(),
            workItem.CreatedAt,
            workItem.UpdatedAt);
    }
}