using Application.Backlogs.Queries.GetBacklog;
using Asp.Versioning;
using Contracts.Backlogs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Constants;

namespace WebApi.Controllers;

[ApiVersion(ApiVersions.V1)]
[Route("api/teams/{teamId:guid}/backlogs")]
public sealed class BacklogsController : ApiController
{
    private readonly ISender _sender;

    public BacklogsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Retrieve the backlog for a team.
    /// </summary>
    /// <param name="teamId">The team's unique identifier.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<BacklogResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<BacklogResponse>>> GetBacklog(
        Guid teamId,
        [FromQuery] GetBacklogRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetBacklogQuery(
            teamId,
            request.SprintId,
            request.BacklogLevel);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }
}