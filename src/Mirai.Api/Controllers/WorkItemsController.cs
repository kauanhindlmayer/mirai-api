using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.WorkItems.Commands.AddComment;
using Mirai.Application.WorkItems.Commands.AssignWorkItem;
using Mirai.Application.WorkItems.Commands.CreateWorkItem;
using Mirai.Application.WorkItems.Queries.GetWorkItem;
using Mirai.Application.WorkItems.Queries.ListWorkItems;
using Mirai.Contracts.WorkItems;
using Mirai.Domain.WorkItems;

namespace Mirai.Api.Controllers;

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
    public async Task<IActionResult> GetWorkItem(Guid workItemId)
    {
        var query = new GetWorkItemQuery(workItemId);

        var result = await _mediator.Send(query);

        return result.Match(
            workItem => Ok(ToDto(workItem)),
            Problem);
    }

    /// <summary>
    /// Add a comment to a work item.
    /// </summary>
    /// <param name="workItemId">The ID of the work item to add a comment to.</param>
    /// <param name="request">The details of the comment to add.</param>
    [HttpPost(ApiEndpoints.WorkItems.AddComment)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddComment(Guid workItemId, AddCommentRequest request)
    {
        var command = new AddCommentCommand(workItemId, request.Content);

        var result = await _mediator.Send(command);

        return result.Match(
            _ => CreatedAtAction(
                actionName: nameof(GetWorkItem),
                routeValues: new { WorkItemId = workItemId },
                value: null),
            Problem);
    }

    /// <summary>
    /// List all work items belonging to a project.
    /// </summary>
    /// <param name="projectId">The ID of the project to list work items for.</param>
    [HttpGet(ApiEndpoints.Projects.ListWorkItems)]
    [ProducesResponseType(typeof(IEnumerable<WorkItemResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListWorkItems(Guid projectId)
    {
        var query = new ListWorkItemsQuery(projectId);

        var result = await _mediator.Send(query);

        return result.Match(
            workItems => Ok(workItems.ConvertAll(ToDto)),
            Problem);
    }

    /// <summary>
    /// Assign a work item to a user.
    /// </summary>
    /// <param name="workItemId">The ID of the work item to assign.</param>
    /// <param name="request">The details of the assignment.</param>
    [HttpPatch(ApiEndpoints.WorkItems.Assign)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AssignWorkItem(Guid workItemId, AssignWorkItemRequest request)
    {
        var command = new AssignWorkItemCommand(workItemId, request.AssigneeId);
        var result = await _mediator.Send(command);
        return result.Match(_ => NoContent(), Problem);
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
            workItem.Comments.Select(ToDto).ToList(),
            workItem.CreatedAt,
            workItem.UpdatedAt);
    }

    private static CommentResponse ToDto(WorkItemComment comment)
    {
        return new(comment.Id, comment.Content, comment.CreatedAt);
    }
}