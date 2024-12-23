using Application.Tags.Commands.CreateTag;
using Application.Tags.Commands.DeleteTag;
using Application.Tags.Commands.UpdateTag;
using Application.Tags.Queries.ListTags;
using Asp.Versioning;
using Contracts.Tags;
using Domain.Tags;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/projects/{projectId:guid}/tags")]
public class TagsController(ISender sender) : ApiController
{
    /// <summary>
    /// Add a tag to a project that can be used to categorize work items.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="request">The tag data.</param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateTag(
        Guid projectId,
        CreateTagRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateTagCommand(projectId, request.Name);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// List all tags in a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="searchTerm">The search term to filter tags by name.</param>
    [HttpGet]
    [ProducesResponseType(typeof(List<TagResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListTags(
        Guid projectId,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = new ListTagsQuery(projectId, searchTerm);

        var result = await sender.Send(query, cancellationToken);

        return result.Match(
            tags => Ok(tags.ConvertAll(ToDto)),
            Problem);
    }

    /// <summary>
    /// Update a tag in a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="tagId">The tag ID.</param>
    /// <param name="request">The new tag data.</param>
    [HttpPut("{tagId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTag(
        Guid projectId,
        Guid tagId,
        CreateTagRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTagCommand(projectId, tagId, request.Name);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Delete a tag from a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="tagName">The tag name.</param>
    [HttpDelete("{tagName}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTag(
        Guid projectId,
        string tagName,
        CancellationToken cancellationToken)
    {
        var command = new DeleteTagCommand(projectId, tagName);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    private static TagResponse ToDto(Tag tag)
    {
        return new(
            tag.Id,
            tag.Name,
            tag.CreatedAt,
            tag.UpdatedAt);
    }
}