using Application.WikiPages.Commands.AddComment;
using Application.WikiPages.Commands.CreateWikiPage;
using Application.WikiPages.Commands.DeleteWikiPage;
using Application.WikiPages.Commands.MoveWikiPage;
using Application.WikiPages.Commands.UpdateWikiPage;
using Application.WikiPages.Queries.GetWikiPage;
using Application.WikiPages.Queries.ListWikiPages;
using Asp.Versioning;
using Contracts.Common;
using Contracts.WikiPages;
using Domain.Users;
using Domain.WikiPages;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/projects/{projectId:guid}/wiki-pages")]
public class WikiPagesController(ISender sender) : ApiController
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

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            wikiPage => CreatedAtAction(
                actionName: nameof(GetWikiPage),
                routeValues: new { ProjectId = projectId, WikiPageId = wikiPage.Id },
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
    public async Task<IActionResult> GetWikiPage(
        Guid wikiPageId,
        CancellationToken cancellationToken)
    {
        var query = new GetWikiPageQuery(wikiPageId);

        var result = await sender.Send(query, cancellationToken);

        return result.Match(
            wikiPage => Ok(ToDto(wikiPage)),
            Problem);
    }

    /// <summary>
    /// List all wiki pages in a project.
    /// </summary>
    /// <param name="projectId">The ID of the project to list wiki pages for.</param>
    [HttpGet]
    [ProducesResponseType(typeof(List<WikiPageSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListWikiPages(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var query = new ListWikiPagesQuery(projectId);

        var result = await sender.Send(query, cancellationToken);

        return result.Match(
            wikiPages => Ok(wikiPages.ConvertAll(ToSummaryDto)),
            Problem);
    }

    /// <summary>
    /// Add a comment to a wiki page.
    /// </summary>
    /// <param name="projectId">The ID of the project to add a comment to.</param>
    /// <param name="wikiPageId">The ID of the wiki page to add a comment to.</param>
    /// <param name="request">The details of the comment to add.</param>
    [HttpPost("{wikiPageId:guid}/comments")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddComment(
        Guid projectId,
        Guid wikiPageId,
        AddCommentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddCommentCommand(wikiPageId, request.Content);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            _ => CreatedAtAction(
                actionName: nameof(GetWikiPage),
                routeValues: new { ProjectId = projectId, WikiPageId = wikiPageId },
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
    public async Task<IActionResult> DeleteWikiPage(
        Guid wikiPageId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteWikiPageCommand(wikiPageId);

        var result = await sender.Send(command, cancellationToken);

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
    public async Task<IActionResult> UpdateWikiPage(
        Guid wikiPageId,
        UpdateWikiPageRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateWikiPageCommand(
            wikiPageId,
            request.Title,
            request.Content);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            wikiPage => Ok(ToDto(wikiPage)),
            Problem);
    }

    /// <summary>
    /// Move a wiki page to a new parent wiki page.
    /// </summary>
    /// <param name="projectId">The ID of the project the wiki page belongs to.</param>
    /// <param name="wikiPageId">The ID of the wiki page to move.</param>
    /// <param name="request">The details of the move operation.</param>
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

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
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
            ToDto(comment.Author),
            comment.Content,
            comment.CreatedAt,
            comment.UpdatedAt);
    }

    private static AuthorResponse ToDto(User author)
    {
        return new(author.FullName);
    }

    private static WikiPageSummaryResponse ToSummaryDto(WikiPage wikiPage)
    {
        return new(
            wikiPage.Id,
            wikiPage.Title,
            wikiPage.SubWikiPages.Select(ToSummaryDto).ToList());
    }
}