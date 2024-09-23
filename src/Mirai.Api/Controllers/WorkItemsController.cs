using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.WorkItems.Commands.AddComment;
using Mirai.Application.WorkItems.Commands.AddTag;
using Mirai.Application.WorkItems.Commands.AssignWorkItem;
using Mirai.Application.WorkItems.Commands.CreateWorkItem;
using Mirai.Application.WorkItems.Commands.RemoveTag;
using Mirai.Application.WorkItems.Queries.GetWorkItem;
using Mirai.Application.WorkItems.Queries.ListWorkItems;
using Mirai.Contracts.Tags;
using Mirai.Contracts.WorkItems;
using Mirai.Domain.WorkItems;
using DomainWorkItemStatus = Mirai.Domain.WorkItems.Enums.WorkItemStatus;
using DomainWorkItemType = Mirai.Domain.WorkItems.Enums.WorkItemType;
using WorkItemStatus = Mirai.Contracts.Common.WorkItemStatus;
using WorkItemType = Mirai.Contracts.Common.WorkItemType;

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
        if (!DomainWorkItemType.TryFromName(request.Type.ToString(), out var type))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: "Invalid work item type");
        }

        var command = new CreateWorkItemCommand(
            ProjectId: request.ProjectId,
            Type: type,
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

    /// <summary>
    /// Add a tag to a work item. If the tag does not exist at project level, it will be created.
    /// </summary>
    /// <param name="workItemId">The ID of the work item to add a tag to.</param>
    /// <param name="request">The details of the tag to add.</param>
    [HttpPost(ApiEndpoints.WorkItems.AddTag)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddTagToWorkItem(Guid workItemId, AddTagToWorkItemRequest request)
    {
        var command = new AddTagCommand(workItemId, request.TagName);
        var result = await _mediator.Send(command);
        return result.Match(_ => NoContent(), Problem);
    }

    /// <summary>
    /// Remove a tag from a work item.
    /// </summary>
    /// <param name="workItemId>">The ID of the work item to remove a tag from.</param>
    /// <param name="tagName">The name of the tag to remove.</param>
    [HttpDelete(ApiEndpoints.WorkItems.RemoveTag)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveTagFromWorkItem(Guid workItemId, string tagName)
    {
        var command = new RemoveTagCommand(workItemId, tagName);
        var result = await _mediator.Send(command);
        return result.Match(_ => NoContent(), Problem);
    }

    private static WorkItemResponse ToDto(WorkItem workItem)
    {
        return new(
            Id: workItem.Id,
            ProjectId: workItem.ProjectId,
            Code: workItem.Code,
            Title: workItem.Title,
            Description: workItem.Description,
            AcceptanceCriteria: workItem.AcceptanceCriteria,
            Status: ToDto(workItem.Status),
            Type: ToDto(workItem.Type),
            Comments: workItem.Comments.Select(ToDto).ToList(),
            Tags: workItem.Tags.Select(t => t.Name).ToList(),
            CreatedAt: workItem.CreatedAt,
            UpdatedAt: workItem.UpdatedAt);
    }

    private static WorkItemType ToDto(DomainWorkItemType workItemType)
    {
        return workItemType.Name switch
        {
            nameof(DomainWorkItemType.UserStory) => WorkItemType.UserStory,
            nameof(DomainWorkItemType.Bug) => WorkItemType.Bug,
            nameof(DomainWorkItemType.Defect) => WorkItemType.Defect,
            nameof(DomainWorkItemType.Epic) => WorkItemType.Epic,
            nameof(DomainWorkItemType.Feature) => WorkItemType.Feature,
            _ => throw new InvalidOperationException(),
        };
    }

    private static WorkItemStatus ToDto(DomainWorkItemStatus workItemStatus)
    {
        return workItemStatus.Name switch
        {
            nameof(DomainWorkItemStatus.New) => WorkItemStatus.New,
            nameof(DomainWorkItemStatus.InProgress) => WorkItemStatus.InProgress,
            nameof(DomainWorkItemStatus.Closed) => WorkItemStatus.Closed,
            nameof(DomainWorkItemStatus.Resolved) => WorkItemStatus.Resolved,
            nameof(DomainWorkItemStatus.Reopened) => WorkItemStatus.Reopened,
            _ => throw new InvalidOperationException(),
        };
    }

    private static CommentResponse ToDto(WorkItemComment comment)
    {
        return new(comment.Id, comment.Content, comment.CreatedAt);
    }
}