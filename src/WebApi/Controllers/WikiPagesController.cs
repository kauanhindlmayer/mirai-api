using Application.WikiPages.Commands.AddComment;
using Application.WikiPages.Commands.CreateWikiPage;
using Application.WikiPages.Commands.DeleteComment;
using Application.WikiPages.Commands.DeleteWikiPage;
using Application.WikiPages.Commands.MoveWikiPage;
using Application.WikiPages.Commands.UpdateWikiPage;
using Application.WikiPages.Queries.GetWikiPage;
using Application.WikiPages.Queries.GetWikiPageStats;
using Application.WikiPages.Queries.ListWikiPages;
using Asp.Versioning;
using Contracts.Common;
using Contracts.WikiPages;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/projects/{projectId:guid}/wiki-pages")]
public class WikiPagesController : ApiController
{
    private readonly ISender _sender;

    public WikiPagesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Create a wiki page.
    /// </summary>
    /// <remarks>
    /// Creates a root wiki page or a sub-page if a ParentWikiPageId is provided.
    /// </remarks>
    /// <param name="projectId">The project's unique identifier.</param>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateWikiPage(
        Guid projectId,
        CreateWikiPageRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateWikiPageCommand(
            projectId,
            request.Title,
            request.Content,
            request.ParentWikiPageId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            wikiPageId => CreatedAtAction(
                nameof(GetWikiPage),
                new { projectId, wikiPageId },
                wikiPageId),
            Problem);
    }

    /// <summary>
    /// Retrieve a wiki page.
    /// </summary>
    /// <remarks>
    /// Retrieves the wiki page with the specified unique identifier.
    /// </remarks>
    /// <param name="wikiPageId">The wiki page's unique identifier.</param>
    [HttpGet("{wikiPageId:guid}")]
    [ProducesResponseType(typeof(WikiPageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWikiPage(
        Guid wikiPageId,
        CancellationToken cancellationToken)
    {
        var query = new GetWikiPageQuery(wikiPageId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Retrieve stats for a wiki page.
    /// </summary>
    /// <remarks>
    /// Retrieves the stats for the wiki page with the specified unique identifier.
    /// </remarks>
    /// <param name="wikiPageId">The wiki page's unique identifier.</param>
    [HttpGet("{wikiPageId:guid}/stats")]
    [ProducesResponseType(typeof(WikiPageStatsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWikiPageStats(
        Guid wikiPageId,
        [FromQuery] GetWikiPageStatsRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetWikiPageStatsQuery(
            wikiPageId,
            request.PageViewsForDays);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Retrieve all wiki pages for a project.
    /// </summary>
    /// <remarks>
    /// Returns a list of wiki pages for the specified project.
    /// </remarks>
    /// <param name="projectId">The project's unique identifier.</param>
    [HttpGet]
    [ProducesResponseType(typeof(List<WikiPageBriefResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListWikiPages(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var query = new ListWikiPagesQuery(projectId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Add a comment to a wiki page.
    /// </summary>
    /// <remarks>
    /// Adds a comment to the specified wiki page.
    /// </remarks>
    /// <param name="projectId">The project's unique identifier.</param>
    /// <param name="wikiPageId">The wiki page's unique identifier.</param>
    [HttpPost("{wikiPageId:guid}/comments")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddCommentToWikiPage(
        Guid projectId,
        Guid wikiPageId,
        AddCommentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddCommentCommand(wikiPageId, request.Content);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            commentId => CreatedAtAction(
                nameof(GetWikiPage),
                new { projectId, wikiPageId },
                commentId),
            Problem);
    }

    /// <summary>
    /// Delete a comment from a wiki page.
    /// </summary>
    /// <remarks>
    /// Deletes the comment with the specified unique identifier from the wiki page.
    /// </remarks>
    /// <param name="wikiPageId">The wiki page's unique identifier.</param>
    /// <param name="commentId">The comment's unique identifier.</param>
    [HttpDelete("{wikiPageId:guid}/comments/{commentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteComment(
        Guid wikiPageId,
        Guid commentId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCommentCommand(wikiPageId, commentId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Delete a wiki page.
    /// </summary>
    /// <remarks>
    /// Deletes the wiki page with the specified unique identifier. Deleting is only
    /// possible if the wiki page does not have any sub-pages associated with it.
    /// </remarks>
    /// <param name="wikiPageId">The wiki page's unique identifier.</param>
    [HttpDelete("{wikiPageId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteWikiPage(
        Guid wikiPageId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteWikiPageCommand(wikiPageId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Update a wiki page.
    /// </summary>
    /// <remarks>
    /// Updates the specified wiki page by settings the values of the parameters passed.
    /// Any parameters not provided will be left unchanged.
    /// </remarks>
    /// <param name="wikiPageId">The wiki page's unique identifier.</param>
    [HttpPut("{wikiPageId:guid}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateWikiPage(
        Guid wikiPageId,
        UpdateWikiPageRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateWikiPageCommand(
            wikiPageId,
            request.Title,
            request.Content);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => Ok(wikiPageId),
            Problem);
    }

    /// <summary>
    /// Move a wiki page to a new position or parent.
    /// </summary>
    /// <remarks>
    /// Moves the wiki page with the specified unique identifier to a new position or parent.
    /// </remarks>
    /// <param name="projectId">The project's unique identifier.</param>
    /// <param name="wikiPageId">The wiki page's unique identifier.</param>
    [HttpPut("{wikiPageId:guid}/move")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MoveWikiPage(
        Guid projectId,
        Guid wikiPageId,
        MoveWikiPageRequest request,
        CancellationToken cancellationToken)
    {
        var command = new MoveWikiPageCommand(
            projectId,
            wikiPageId,
            request.TargetParentId,
            request.TargetPosition);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }
}