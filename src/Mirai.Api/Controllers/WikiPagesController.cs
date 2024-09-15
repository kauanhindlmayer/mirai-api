using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.WikiPages.Commands.AddComment;
using Mirai.Application.WikiPages.Commands.CreateWikiPage;
using Mirai.Application.WikiPages.Commands.DeleteWikiPage;
using Mirai.Application.WikiPages.Queries.GetWikiPage;
using Mirai.Application.WikiPages.Queries.ListWikiPages;
using Mirai.Contracts.WikiPages;
using Mirai.Contracts.WorkItems;
using Mirai.Domain.WikiPages;

namespace Mirai.Api.Controllers;

public class WikiPagesController(ISender _mediator) : ApiController
{
    /// <summary>
    /// Creates a new wiki page. If a ParentWikiPageId is provided, the page
    /// will be created as a sub-page under the specified parent wiki page.
    /// </summary>
    /// <param name="request">The details of the wiki page to create.</param>
    [HttpPost(ApiEndpoints.WikiPages.Create)]
    [ProducesResponseType(typeof(WikiPageDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateWikiPage(CreateWikiPageRequest request)
    {
        var command = new CreateWikiPageCommand(
            ProjectId: request.ProjectId,
            Title: request.Title,
            Content: request.Content,
            ParentWikiPageId: request.ParentWikiPageId);

        var result = await _mediator.Send(command);

        return result.Match(
            wikiPage => CreatedAtAction(
                actionName: nameof(GetWikiPage),
                routeValues: new { WikiPageId = wikiPage.Id },
                value: ToDto(wikiPage)),
            Problem);
    }

    /// <summary>
    /// Get a wiki page by its ID.
    /// </summary>
    /// <param name="wikiPageId">The ID of the wiki page to get.</param>
    [HttpGet(ApiEndpoints.WikiPages.Get)]
    [ProducesResponseType(typeof(WikiPageDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWikiPage(Guid wikiPageId)
    {
        var query = new GetWikiPageQuery(wikiPageId);

        var result = await _mediator.Send(query);

        return result.Match(
            wikiPage => Ok(ToDto(wikiPage)),
            Problem);
    }

    /// <summary>
    /// List all wiki pages in a project.
    /// </summary>
    /// <param name="projectId">The ID of the project to list wiki pages for.</param>
    [HttpGet(ApiEndpoints.WikiPages.List)]
    [ProducesResponseType(typeof(IEnumerable<WikiPageSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListWikiPages(Guid projectId)
    {
        var query = new ListWikiPagesQuery(projectId);

        var result = await _mediator.Send(query);

        return result.Match(
            wikiPages => Ok(wikiPages.ConvertAll(ToSummaryDto)),
            Problem);
    }

    /// <summary>
    /// Add a comment to a wiki page.
    /// </summary>
    /// <param name="wikiPageId">The ID of the wiki page to add a comment to.</param>
    /// <param name="request">The details of the comment to add.</param>
    [HttpPost(ApiEndpoints.WikiPages.AddComment)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddComment(Guid wikiPageId, AddCommentRequest request)
    {
        var command = new AddCommentCommand(wikiPageId, request.Content);

        var result = await _mediator.Send(command);

        return result.Match(
            _ => CreatedAtAction(
                actionName: nameof(GetWikiPage),
                routeValues: new { WikiPageId = wikiPageId },
                value: null),
            Problem);
    }

    /// <summary>
    /// Delete a wiki page by its ID.
    /// </summary>
    /// <param name="wikiPageId">The ID of the wiki page to delete.</param>
    [HttpDelete(ApiEndpoints.WikiPages.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteWikiPage(Guid wikiPageId)
    {
        var command = new DeleteWikiPageCommand(wikiPageId);
        var result = await _mediator.Send(command);
        return result.Match(_ => NoContent(), Problem);
    }

    private static WikiPageDetailResponse ToDto(WikiPage wikiPage)
    {
        return new(
            Id: wikiPage.Id,
            ProjectId: wikiPage.ProjectId,
            Title: wikiPage.Title,
            Content: wikiPage.Content,
            Comments: wikiPage.Comments.Select(ToCommentDto).ToList(),
            CreatedAt: wikiPage.CreatedAt,
            UpdatedAt: wikiPage.UpdatedAt);
    }

    private static WikiPageCommentResponse ToCommentDto(WikiPageComment comment)
    {
        return new(
            Id: comment.Id,
            UserId: comment.UserId,
            Content: comment.Content,
            CreatedAt: comment.CreatedAt,
            UpdatedAt: comment.UpdatedAt);
    }

    private static WikiPageSummaryResponse ToSummaryDto(WikiPage wikiPage)
    {
        return new(
            Id: wikiPage.Id,
            Title: wikiPage.Title,
            SubPages: wikiPage.SubWikiPages.Select(ToSummaryDto).ToList());
    }
}