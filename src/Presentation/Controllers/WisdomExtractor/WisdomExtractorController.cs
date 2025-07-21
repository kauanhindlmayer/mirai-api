using System.Net.Mime;
using Application.WisdomExtractor.Queries.ExtractWisdom;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Constants;

namespace Presentation.Controllers.WisdomExtractor;

[ApiVersion(ApiVersions.V1)]
[Route("api/projects/{projectId:guid}/wisdom-extractor")]
[Produces(MediaTypeNames.Application.Json)]
public sealed class WisdomExtractorController : ApiController
{
    private readonly ISender _sender;

    public WisdomExtractorController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Extract wisdom from a given question.
    /// </summary>
    /// <remarks>
    /// This endpoint processes a natural-language question to generate focused answers
    /// and source excerpts from the project's work items and wiki pages.
    /// </remarks>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <param name="request">The request containing the question to be answered.</param>
    [HttpPost]
    [ProducesResponseType(typeof(WisdomResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ExtractWisdom(
        Guid projectId,
        ExtractWisdomRequest request,
        CancellationToken cancellationToken)
    {
        var query = new ExtractWisdomQuery(
            projectId,
            request.Question);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }
}