using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.WikiPages.Commands.CreateWikiPage;
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
    [ProducesResponseType(typeof(WikiPageResponse), StatusCodes.Status201Created)]
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

    private static WikiPageResponse ToDto(WikiPage wikiPage)
    {
        return new(
            Id: wikiPage.Id,
            ProjectId: wikiPage.ProjectId,
            Title: wikiPage.Title,
            Content: wikiPage.Content,
            CreatedAt: wikiPage.CreatedAt,
            UpdatedAt: wikiPage.UpdatedAt);
    }
}