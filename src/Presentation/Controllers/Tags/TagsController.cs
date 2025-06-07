using System.Net.Mime;
using Application.Common;
using Application.Tags.Commands.CreateTag;
using Application.Tags.Commands.DeleteTag;
using Application.Tags.Commands.DeleteTags;
using Application.Tags.Commands.MergeTags;
using Application.Tags.Commands.UpdateTag;
using Application.Tags.Queries.ExportTags;
using Application.Tags.Queries.ListTags;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Constants;

namespace Presentation.Controllers.Tags;

[ApiVersion(ApiVersions.V1)]
[Route("api/projects/{projectId:guid}/tags")]
[Produces(MediaTypeNames.Application.Json, CustomMediaTypeNames.Application.JsonV1)]
public sealed class TagsController : ApiController
{
    private readonly ISender _sender;

    public TagsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Create a tag.
    /// </summary>
    /// <remarks>
    /// Tags are used to categorize work items. They can be used to filter and
    /// search for work items in the project.
    /// </remarks>
    /// <param name="projectId">The project's unique identifier.</param>
    /// <returns>The unique identifier of the created tag.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> CreateTag(
        Guid projectId,
        CreateTagRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateTagCommand(
            projectId,
            request.Name,
            request.Description,
            request.Color);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            tagId => Ok(tagId),
            Problem);
    }

    /// <summary>
    /// Retrieve a paginated list of tags for a project.
    /// </summary>
    /// <param name="projectId">The project's unique identifier.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<TagResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<TagResponse>>> ListTags(
        Guid projectId,
        [FromQuery] PageRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = new ListTagsQuery(
            projectId,
            request.Page,
            request.PageSize,
            request.Sort,
            request.SearchTerm);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Update a tag.
    /// </summary>
    /// <param name="projectId">The project's unique identifier.</param>
    /// <param name="tagId">The tag's unique identifier.</param>
    /// <returns>The unique identifier of the updated tag.</returns>
    [HttpPut("{tagId:guid}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> UpdateTag(
        Guid projectId,
        Guid tagId,
        UpdateTagRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTagCommand(
            projectId,
            tagId,
            request.Name,
            request.Description,
            request.Color);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => Ok(tagId),
            Problem);
    }

    /// <summary>
    /// Delete a tag.
    /// </summary>
    /// <param name="projectId">The project's unique identifier.</param>
    /// <param name="tagId">The tag's unique identifier.</param>
    [HttpDelete("{tagId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteTag(
        Guid projectId,
        Guid tagId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteTagCommand(projectId, tagId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Delete multiple tags from a project.
    /// </summary>
    /// <remarks>
    /// This operation allows for bulk deletion of tags. It is useful for
    /// cleaning up multiple tags at once.
    /// </remarks>
    /// <param name="projectId">The project's unique identifier.</param>
    [HttpDelete("bulk")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteTags(
        Guid projectId,
        [FromBody] DeleteTagsRequest request,
        CancellationToken cancellationToken)
    {
        var command = new DeleteTagsCommand(projectId, request.TagIds);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Merge multiple tags into a target tag within a project.
    /// </summary>
    /// <remarks>
    /// This operation transfers all work item associations from the source tags
    /// to the target tag, and then deletes the source tags. Useful for cleaning up
    /// duplicate or obsolete tags. The target tag must belong to the same project,
    /// and cannot be included as a source.
    /// </remarks>
    /// <param name="projectId">The project's unique identifier.</param>
    [HttpPost("merge")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> MergeTags(
        Guid projectId,
        MergeTagsRequest request,
        CancellationToken cancellationToken)
    {
        var command = new MergeTagsCommand(
            projectId,
            request.TargetTagId,
            request.SourceTagIds);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Export all tags for a project as a CSV file.
    /// </summary>
    /// <param name="projectId">The project's unique identifier.</param>
    [HttpGet("export")]
    [Produces("text/csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ExportTags(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var query = new ExportTagsQuery(projectId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(
            csv => File(csv, "text/csv", "tags.csv"),
            Problem);
    }
}