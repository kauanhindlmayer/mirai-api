using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.WikiPages.Commands.CreateWikiPage;
using Mirai.Application.WikiPages.Queries.GetWikiPage;
using Mirai.Application.WikiPages.Queries.ListWikiPages;
using Mirai.Contracts.WikiPages;
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