using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.WikiPages.Commands.CreateWikiPage;
using Mirai.Application.WikiPages.Queries.GetWikiPage;
using Mirai.Contracts.WikiPages;
using Mirai.Domain.WikiPages;

namespace Mirai.Api.Controllers;

public class WikiPagesController(ISender _mediator) : ApiController
{
    /// <summary>
    /// Create a new wiki page.
    /// </summary>
    /// <param name="request">The details of the wiki page to create.</param>
    [HttpPost(ApiEndpoints.WikiPages.Create)]
    [ProducesResponseType(typeof(WikiPageDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateWorkItem(CreateWikiPageRequest request)
    {
        var command = new CreateWikiPageCommand(
            ProjectId: request.ProjectId,
            Title: request.Title,
            Content: request.Content);

        var result = await _mediator.Send(command);

        return result.Match(
            workItem => CreatedAtAction(
                actionName: "GetWikiPage", // nameof(GetWikiPage),
                routeValues: new { WikiPageId = workItem.Id },
                value: ToDto(workItem)),
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
            workItem => Ok(ToDto(workItem)),
            Problem);
    }

    private static WikiPageDetailResponse ToDto(WikiPage wikiPage)
    {
        return new(
            Id: wikiPage.Id,
            ProjectId: wikiPage.ProjectId,
            Title: wikiPage.Title,
            Content: wikiPage.Content,
            CreatedAt: wikiPage.CreatedAt,
            UpdatedAt: wikiPage.UpdatedAt);
    }

    private static WikiPageSummaryResponse ToSummaryDto(WikiPage wikiPage)
    {
        return new(
            Id: wikiPage.Id,
            ProjectId: wikiPage.ProjectId,
            Title: wikiPage.Title);
    }
}