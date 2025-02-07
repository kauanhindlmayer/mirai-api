using Application.Common;
using Application.WorkItems.Commands.AddComment;
using Application.WorkItems.Commands.AddTag;
using Application.WorkItems.Commands.AssignWorkItem;
using Application.WorkItems.Commands.CreateWorkItem;
using Application.WorkItems.Commands.DeleteWorkItem;
using Application.WorkItems.Commands.RemoveTag;
using Application.WorkItems.Queries.Common;
using Application.WorkItems.Queries.GetWorkItem;
using Application.WorkItems.Queries.GetWorkItemsStats;
using Application.WorkItems.Queries.ListWorkItems;
using Application.WorkItems.Queries.SearchWorkItems;
using Asp.Versioning;
using Contracts.Common;
using Contracts.Tags;
using Contracts.WorkItems;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/projects/{projectId:guid}/work-items")]
public class WorkItemsController : ApiController
{
    private readonly ISender _sender;

    public WorkItemsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Create a work item.
    /// </summary>
    /// <remarks>
    /// Creates a new work item for the specified project.
    /// </remarks>
    /// <param name="projectId">The project's unique identifier.</param>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateWorkItem(
        Guid projectId,
        CreateWorkItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateWorkItemCommand(
            projectId,
            request.Type,
            request.Title,
            request.AssignedTeamId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            workItemId => CreatedAtAction(
                nameof(GetWorkItem),
                new { projectId, workItemId },
                workItemId),
            Problem);
    }

    /// <summary>
    /// Retrieve a work item.
    /// </summary>
    /// <remarks>
    /// Retrieves the work item with the specified unique identifier.
    /// </remarks>
    /// <param name="workItemId">The work item's unique identifier.</param>
    [HttpGet("{workItemId:guid}")]
    [ProducesResponseType(typeof(WorkItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWorkItem(
        Guid workItemId,
        CancellationToken cancellationToken)
    {
        var query = new GetWorkItemQuery(workItemId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Retrieve statistics for work items in a project.
    /// </summary>
    /// <remarks>
    /// Retrieves statistics for work items in the specified project.
    /// </remarks>
    /// <param name="projectId">The project's unique identifier.</param>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(WorkItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWorkItemsStats(
        Guid projectId,
        [FromQuery] GetWorkItemsStatsRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetWorkItemsStatsQuery(
            projectId,
            request.PeriodInDays);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Add a comment to a work item.
    /// </summary>
    /// <remarks>
    /// Adds a comment to the work item with the specified unique identifier.
    /// </remarks>
    /// <param name="projectId">The project's unique identifier.</param>
    /// <param name="workItemId">The work item's unique identifier.</param>
    [HttpPost("{workItemId:guid}/comments")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddCommentToWorkItem(
        Guid projectId,
        Guid workItemId,
        AddCommentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddCommentCommand(workItemId, request.Content);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            commentId => CreatedAtAction(
                nameof(GetWorkItem),
                new { projectId, workItemId },
                commentId),
            Problem);
    }

    /// <summary>
    /// Retrieve all work items in a project.
    /// </summary>
    /// <remarks>
    /// Support for pagination, sorting, and searching.
    /// </remarks>
    /// <param name="projectId">The ID of the project.</param>
    /// <param name="request">The details of the page.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<WorkItemBriefResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListWorkItems(
        Guid projectId,
        [FromQuery] PageRequest request,
        CancellationToken cancellationToken)
    {
        var query = new ListWorkItemsQuery(
            projectId,
            request.PageNumber,
            request.PageSize,
            request.SortField,
            request.SortOrder,
            request.SearchTerm);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Assign a work item to a user.
    /// </summary>
    /// <remarks>
    /// Assigns the work item with the specified unique identifier to the user
    /// with the specified unique identifier.
    /// </remarks>
    /// <param name="workItemId">The work item's unique identifier.</param>
    [HttpPatch("{workItemId:guid}/assign")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AssignWorkItem(
        Guid workItemId,
        AssignWorkItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AssignWorkItemCommand(workItemId, request.AssigneeId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Add a tag to a work item.
    /// </summary>
    /// <remarks>
    /// If the tag does not exist at project level, it will be created.
    /// </remarks>
    /// <param name="workItemId">The work item's unique identifier.</param>
    [HttpPost("{workItemId:guid}/tags")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddTagToWorkItem(
        Guid workItemId,
        AddTagToWorkItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddTagCommand(workItemId, request.Name);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Remove a tag from a work item.
    /// </summary>
    /// <remarks>
    /// Removes the tag with the specified name from the work item with the
    /// specified unique identifier.
    /// </remarks>
    /// <param name="workItemId>">The work item's unique identifier.</param>
    /// <param name="tagName">The tag's name.</param>
    [HttpDelete("{workItemId:guid}/tags/{tagName}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveTagFromWorkItem(
        Guid workItemId,
        string tagName,
        CancellationToken cancellationToken)
    {
        var command = new RemoveTagCommand(workItemId, tagName);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Search for work items in a project.
    /// </summary>
    /// <remarks>
    /// This search analyzes the meaning of the provided term to find relevant work items,
    /// rather than relying solely on exact keyword matches.
    /// </remarks>
    /// <param name="projectId">The project's unique identifier.</param>
    /// <param name="searchTerm">The search term used to find relevant work items.</param>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IReadOnlyList<WorkItemBriefResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchWorkItems(
        Guid projectId,
        string searchTerm,
        CancellationToken cancellationToken)
    {
        var query = new SearchWorkItemsQuery(projectId, searchTerm);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Delete a work item.
    /// </summary>
    /// <remarks>
    /// Deletes the work item with the specified unique identifier.
    /// Deleting is only possible if the work item does not have any related
    /// work items associated with it.
    /// </remarks>
    /// <param name="workItemId">The work item's unique identifier.</param>
    [HttpDelete("{workItemId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteWorkItem(
        Guid workItemId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteWorkItemCommand(workItemId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }
}