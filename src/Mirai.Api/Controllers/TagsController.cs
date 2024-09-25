using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.Tags.Commands.CreateTag;
using Mirai.Application.Tags.Commands.DeleteTag;
using Mirai.Application.Tags.Queries.ListTags;
using Mirai.Contracts.Tags;
using Mirai.Domain.Tags;

namespace Mirai.Api.Controllers;

public class TagsController(ISender _mediator) : ApiController
{
    /// <summary>
    /// Add a tag to a project that can be used to categorize work items.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="request">The tag data.</param>
    [HttpPost(ApiEndpoints.Projects.AddTag)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddTag(Guid projectId, CreateTagRequest request)
    {
        var command = new CreateTagCommand(projectId, request.Name);

        var result = await _mediator.Send(command);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// List all tags in a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    [HttpGet(ApiEndpoints.Projects.ListTags)]
    [ProducesResponseType(typeof(List<TagResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListTags(Guid projectId)
    {
        var query = new ListTagsQuery(projectId);

        var result = await _mediator.Send(query);

        return result.Match(
            tags => Ok(tags.ConvertAll(ToDto)),
            Problem);
    }

    /// <summary>
    /// Update a tag in a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="tagName">The tag name.</param>
    /// <param name="request">The new tag data.</param>
    [HttpPut(ApiEndpoints.Projects.UpdateTag)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTag(Guid projectId, string tagName, CreateTagRequest request)
    {
        // var command = new UpdateTagCommand(projectId, tagName, request.Name);

        // var result = await _mediator.Send(command);

        // return result.Match(
        //     _ => NoContent(),
        //     Problem);
        await Task.CompletedTask;
        return NoContent();
    }

    /// <summary>
    /// Remove a tag from a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="tagName">The tag name.</param>
    [HttpDelete(ApiEndpoints.Projects.RemoveTag)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTag(Guid projectId, string tagName)
    {
        var command = new DeleteTagCommand(projectId, tagName);

        var result = await _mediator.Send(command);

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