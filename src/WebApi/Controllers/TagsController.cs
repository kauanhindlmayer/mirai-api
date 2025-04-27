using Application.Tags.Commands.CreateTag;
using Application.Tags.Commands.DeleteTag;
using Application.Tags.Commands.UpdateTag;
using Application.Tags.Queries.ListTags;
using Asp.Versioning;
using Contracts.Tags;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/projects/{projectId:guid}/tags")]
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
    /// Creates a new tag for the specified project. This tag can be used to
    /// categorize work items.
    /// </remarks>
    /// <param name="projectId">The project's unique identifier.</param>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateTag(
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
    /// Retrieve all tags for a project.
    /// </summary>
    /// <remarks>
    /// Returns a list of tags for the specified project. Tags can be used to
    /// categorize work items.
    /// </remarks>
    /// <param name="projectId">The project's unique identifier.</param>
    /// <param name="searchTerm">The search term to filter tags by (optional).</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TagResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListTags(
        Guid projectId,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = new ListTagsQuery(projectId, searchTerm);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Update a tag.
    /// </summary>
    /// <remarks>
    /// Updates the specified tag by settings the values of the parameters passed.
    /// Any parameters not provided will be left unchanged.
    /// </remarks>
    /// <param name="projectId">The project's unique identifier.</param>
    /// <param name="tagId">The tag's unique identifier.</param>
    [HttpPut("{tagId:guid}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTag(
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
    /// <remarks>
    /// Deletes the tag with the specified unique identifier.
    /// </remarks>
    /// <param name="projectId">The project's unique identifier.</param>
    /// <param name="tagId">The tag's unique identifier.</param>
    [HttpDelete("{tagId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTag(
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
}