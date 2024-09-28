using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.WikiPages.Commands.AddComment;
using Mirai.Application.WikiPages.Commands.CreateWikiPage;
using Mirai.Application.WikiPages.Commands.DeleteWikiPage;
using Mirai.Application.WikiPages.Commands.UpdateWikiPage;
using Mirai.Application.WikiPages.Queries.GetWikiPage;
using Mirai.Application.WikiPages.Queries.ListWikiPages;
using Mirai.Contracts.WikiPages;
using Mirai.Contracts.WorkItems;
using Mirai.Domain.WikiPages;

namespace Mirai.Api.Controllers;

[Route("api/projects/{projectId:guid}/wiki-pages")]
public class WikiPagesController(ISender _mediator) : ApiController
{
    /// <summary>
    /// Creates a new wiki page. If a ParentWikiPageId is provided, the page
    /// will be created as a sub-page under the specified parent wiki page.
    /// </summary>
    /// <param name="projectId">The ID of the project to create the wiki page in.</param>
    /// <param name="request">The details of the wiki page to create.</param>
    [HttpPost]
    [ProducesResponseType(typeof(WikiPageDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateWikiPage(Guid projectId, CreateWikiPageRequest request)
    {
        var command = new CreateWikiPageCommand(
            projectId,
            request.Title,
            request.Content,
            request.ParentWikiPageId);

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
    [HttpGet("{wikiPageId:guid}")]
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
    [HttpGet]
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
    [HttpPost("{wikiPageId:guid}/comments")]
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
    [HttpDelete("{wikiPageId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteWikiPage(Guid wikiPageId)
    {
        var command = new DeleteWikiPageCommand(wikiPageId);

        var result = await _mediator.Send(command);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Update a wiki page by its ID.
    /// </summary>
    /// <param name="wikiPageId">The ID of the wiki page to update.</param>
    /// <param name="request">The details of the wiki page to update.</param>
    [HttpPut("{wikiPageId:guid}")]
    [ProducesResponseType(typeof(WikiPageDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateWikiPage(Guid wikiPageId, UpdateWikiPageRequest request)
    {
        var command = new UpdateWikiPageCommand(
            wikiPageId,
            request.Title,
            request.Content);

        var result = await _mediator.Send(command);

        return result.Match(
            wikiPage => Ok(ToDto(wikiPage)),
            Problem);
    }

    private static WikiPageDetailResponse ToDto(WikiPage wikiPage)
    {
        return new(
            wikiPage.Id,
            wikiPage.ProjectId,
            wikiPage.Title,
            wikiPage.Content,
            wikiPage.Comments.Select(ToCommentDto).ToList(),
            wikiPage.CreatedAt,
            wikiPage.UpdatedAt);
    }

    private static WikiPageCommentResponse ToCommentDto(WikiPageComment comment)
    {
        return new(
            comment.Id,
            comment.UserId,
            comment.Content,
            comment.CreatedAt,
            comment.UpdatedAt);
    }

    private static WikiPageSummaryResponse ToSummaryDto(WikiPage wikiPage)
    {
        return new(
            wikiPage.Id,
            wikiPage.Title,
            wikiPage.SubWikiPages.Select(ToSummaryDto).ToList());
    }
}