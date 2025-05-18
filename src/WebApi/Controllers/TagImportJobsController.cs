using Application.TagImportJobs.Commands.CreateTagImportJob;
using Application.TagImportJobs.Queries.GetTagImportJobStatus;
using Asp.Versioning;
using Contracts.TagImportJobs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Constants;

namespace WebApi.Controllers;

[ApiVersion(ApiVersions.V1)]
[Route("api/projects/{projectId:guid}/tags/import")]
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
    /// Retrieve the status of a tag import job.
    /// </summary>
    /// <param name="importJobId">The unique identifier of the import job.</param>
    [HttpGet("{importJobId:guid}")]
    [ProducesResponseType(typeof(TagImportJobResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TagImportJobResponse>> GetTagImportJobStatus(
        Guid importJobId,
        CancellationToken cancellationToken)
    {
        var query = new GetTagImportJobStatusQuery(importJobId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }
}