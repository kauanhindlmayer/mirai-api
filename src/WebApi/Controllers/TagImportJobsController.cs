using System.Net.Mime;
using Application.Common;
using Application.TagImportJobs.Commands.CreateTagImportJob;
using Application.TagImportJobs.Queries.GetTagImportJob;
using Application.TagImportJobs.Queries.ListTagImportJobs;
using Asp.Versioning;
using Contracts.TagImportJobs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Constants;

namespace WebApi.Controllers;

[ApiVersion(ApiVersions.V1)]
[Route("api/projects/{projectId:guid}/tags/import")]
[Produces(
    MediaTypeNames.Application.Json,
    CustomMediaTypeNames.Application.JsonV1,
    CustomMediaTypeNames.Application.HateoasJson,
    CustomMediaTypeNames.Application.HateoasJsonV1)]
public sealed class TagImportJobsController : ApiController
{
    private readonly ISender _sender;

    public TagImportJobsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Create a import job for tags.
    /// </summary>
    /// <param name="projectId">The project's unique identifier.</param>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> CreateTagImportJob(
        Guid projectId,
        [FromForm] CreateTagImportJobRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateTagImportJobCommand(projectId, request.File);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            importJobId => Ok(importJobId),
            Problem);
    }

    /// <summary>
    /// Retrieve the status of a specific tag import job.
    /// </summary>
    /// <param name="importJobId">The unique identifier of the import job.</param>
    [HttpGet("{importJobId:guid}")]
    [ProducesResponseType(typeof(TagImportJobResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TagImportJobResponse>> GetTagImportJob(
        Guid importJobId,
        CancellationToken cancellationToken)
    {
        var query = new GetTagImportJobQuery(importJobId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Retrieve a paginated list of tag import jobs for a project.
    /// </summary>
    /// <param name="projectId">The project's unique identifier.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<TagImportJobResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<TagImportJobResponse>>> ListTagImportJobs(
        Guid projectId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new ListTagImportJobsQuery(
            projectId,
            page,
            pageSize);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }
}