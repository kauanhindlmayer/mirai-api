using System.Net.Mime;
using Application.Abstractions;
using Application.TagImportJobs.Commands.CreateTagImportJob;
using Application.TagImportJobs.Queries.GetTagImportJob;
using Application.TagImportJobs.Queries.ListTagImportJobs;
using Asp.Versioning;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Constants;

namespace Presentation.Controllers.TagImportJobs;

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
    private readonly LinkService _linkService;

    public TagImportJobsController(
        ISender sender,
        LinkService linkService)
    {
        _sender = sender;
        _linkService = linkService;
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
    /// <param name="projectId">The project's unique identifier.</param>
    /// <param name="importJobId">The unique identifier of the import job.</param>
    [HttpGet("{importJobId:guid}")]
    [ProducesResponseType(typeof(TagImportJobResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TagImportJobResponse>> GetTagImportJob(
        Guid projectId,
        Guid importJobId,
        [FromHeader] AcceptHeaderRequest acceptHeader,
        CancellationToken cancellationToken)
    {
        var query = new GetTagImportJobQuery(importJobId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(
            importJob =>
            {
                if (acceptHeader.IncludeLinks)
                {
                    importJob.Links = CreateLinksForImportJob(projectId, importJob.Id);
                }

                return Ok(importJob);
            },
            Problem);
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
        [FromHeader] AcceptHeaderRequest acceptHeader,
        CancellationToken cancellationToken)
    {
        var query = new ListTagImportJobsQuery(
            projectId,
            page,
            pageSize);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(
            paginatedList =>
            {
                if (acceptHeader.IncludeLinks)
                {
                    foreach (var importJob in paginatedList.Items)
                    {
                        importJob.Links = CreateLinksForImportJob(projectId, importJob.Id);
                    }

                    paginatedList.Links = CreateLinksForImportJobs(
                        projectId,
                        page,
                        pageSize,
                        paginatedList.HasNextPage,
                        paginatedList.HasPreviousPage);
                }

                return Ok(paginatedList);
            },
            Problem);
    }

    private List<LinkResponse> CreateLinksForImportJob(Guid projectId, Guid id)
    {
        var routeValues = new { projectId, importJobId = id };
        return
        [
            _linkService.Create(nameof(GetTagImportJob), "self", HttpMethods.Get, routeValues)
        ];
    }

    private List<LinkResponse> CreateLinksForImportJobs(
        Guid projectId,
        int page,
        int pageSize,
        bool hasNextPage,
        bool hasPreviousPage)
    {
        var links = new List<LinkResponse>
        {
            CreatePageLink("self", projectId, page, pageSize),
        };

        if (hasNextPage)
        {
            links.Add(CreatePageLink("next-page", projectId, page + 1, pageSize));
        }

        if (hasPreviousPage)
        {
            links.Add(CreatePageLink("previous-page", projectId, page - 1, pageSize));
        }

        return links;
    }

    private LinkResponse CreatePageLink(string rel, Guid projectId, int page, int pageSize)
    {
        var routeValues = new { projectId, page, pageSize };
        return _linkService.Create(nameof(ListTagImportJobs), rel, HttpMethods.Get, routeValues);
    }
}