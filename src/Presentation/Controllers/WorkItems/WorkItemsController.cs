using System.Net.Mime;
using Application.Abstractions;
using Application.WorkItems.Commands.AddComment;
using Application.WorkItems.Commands.AddTag;
using Application.WorkItems.Commands.AssignWorkItem;
using Application.WorkItems.Commands.CreateWorkItem;
using Application.WorkItems.Commands.DeleteWorkItem;
using Application.WorkItems.Commands.LinkWorkItems;
using Application.WorkItems.Commands.RemoveTag;
using Application.WorkItems.Commands.UnlinkWorkItems;
using Application.WorkItems.Commands.UpdateComment;
using Application.WorkItems.Queries.GetWorkItem;
using Application.WorkItems.Queries.GetWorkItemsStats;
using Application.WorkItems.Queries.ListWorkItems;
using Application.WorkItems.Queries.SearchWorkItems;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Constants;
using Presentation.Controllers.Tags;

namespace Presentation.Controllers.WorkItems;

[ApiVersion(ApiVersions.V1)]
[Route("api/projects/{projectId:guid}/work-items")]
[Produces(MediaTypeNames.Application.Json, CustomMediaTypeNames.Application.JsonV1)]
public sealed class WorkItemsController : ApiController
{
    private readonly ISender _sender;

    public WorkItemsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Create a work item.
    /// </summary>
    /// <param name="projectId">The project's unique identifier.</param>
    /// <returns>The unique identifier of the created work item.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateWorkItem(
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
    /// <param name="workItemId">The work item's unique identifier.</param>
    [HttpGet("{workItemId:guid}")]
    [ProducesResponseType(typeof(WorkItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkItemResponse>> GetWorkItem(
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
    /// <param name="projectId">The project's unique identifier.</param>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(WorkItemsStatsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkItemsStatsResponse>> GetWorkItemsStats(
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
    /// <param name="projectId">The project's unique identifier.</param>
    /// <param name="workItemId">The work item's unique identifier.</param>
    /// <returns>The unique identifier of the created comment.</returns>
    [HttpPost("{workItemId:guid}/comments")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> AddCommentToWorkItem(
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
    /// Update a comment on a work item.
    /// </summary>
    /// <param name="workItemId">The work item's unique identifier.</param>
    /// <param name="commentId">The comment's unique identifier.</param>
    [HttpPut("{workItemId:guid}/comments/{commentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateComment(
        Guid workItemId,
        Guid commentId,
        UpdateCommentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCommentCommand(workItemId, commentId, request.Content);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Retrieves a paginated list of work items for the specified project.
    /// </summary>
    /// <param name="projectId">The project's unique identifier.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<WorkItemBriefResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<WorkItemBriefResponse>>> ListWorkItems(
        Guid projectId,
        [FromQuery] WorkItemsQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        var query = new ListWorkItemsQuery(
            projectId,
            parameters.Page,
            parameters.PageSize,
            parameters.Sort,
            parameters.SearchTerm,
            parameters.Type,
            parameters.Status,
            parameters.AssigneeId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Assign a work item to a user.
    /// </summary>
    /// <param name="workItemId">The work item's unique identifier.</param>
    [HttpPatch("{workItemId:guid}/assign")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AssignWorkItem(
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
    /// <param name="projectId">The project's unique identifier.</param>
    /// <param name="workItemId">The work item's unique identifier.</param>
    [HttpPost("{workItemId:guid}/tags")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> AddTagToWorkItem(
        Guid projectId,
        Guid workItemId,
        AddTagToWorkItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddTagCommand(projectId, workItemId, request.Name);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Remove a tag from a work item.
    /// </summary>
    /// <param name="workItemId>">The work item's unique identifier.</param>
    /// <param name="tagName">The tag's name.</param>
    [HttpDelete("{workItemId:guid}/tags/{tagName}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveTagFromWorkItem(
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
    [ProducesResponseType(typeof(IReadOnlyList<WorkItemResponseWithDistance>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<WorkItemResponseWithDistance>>> SearchWorkItems(
        Guid projectId,
        [FromQuery(Name = "q")] string searchTerm,
        CancellationToken cancellationToken)
    {
        var query = new SearchWorkItemsQuery(projectId, searchTerm);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Update a work item.
    /// </summary>
    /// <remarks>
    /// Updates the specific work item by setting the values of the parameters
    /// passed. Any parameters not provided will be left unchanged.
    /// </remarks>
    /// <param name="workItemId">The work item's unique identifier.</param>
    [HttpPut("{workItemId:guid}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateWorkItem(
        Guid workItemId,
        UpdateWorkItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand(workItemId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            workItemId => Ok(workItemId),
            Problem);
    }

    /// <summary>
    /// Delete a work item.
    /// </summary>
    /// <param name="workItemId">The work item's unique identifier.</param>
    [HttpDelete("{workItemId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteWorkItem(
        Guid workItemId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteWorkItemCommand(workItemId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Link two work items with a specific relationship type.
    /// </summary>
    /// <param name="projectId">The project's unique identifier.</param>
    /// <param name="workItemId">The source work item's unique identifier.</param>
    /// <param name="request">The link request containing target work item and link type.</param>
    [HttpPost("{workItemId:guid}/links")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> LinkWorkItems(
        Guid projectId,
        Guid workItemId,
        LinkWorkItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LinkWorkItemsCommand(
            workItemId,
            request.TargetWorkItemId,
            request.LinkType,
            request.Comment);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            linkId => CreatedAtAction(nameof(GetWorkItem), new { projectId, workItemId }, linkId),
            Problem);
    }

    /// <summary>
    /// Remove a link between work items.
    /// </summary>
    /// <param name="workItemId">The source work item's unique identifier.</param>
    /// <param name="linkId">The link's unique identifier.</param>
    [HttpDelete("{workItemId:guid}/links/{linkId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UnlinkWorkItems(
        Guid workItemId,
        Guid linkId,
        CancellationToken cancellationToken)
    {
        var command = new UnlinkWorkItemsCommand(workItemId, linkId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }
}