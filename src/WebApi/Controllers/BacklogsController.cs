using Application.Backlogs.Queries.GetBacklog;
using Asp.Versioning;
using Contracts.Backlogs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/teams/{teamId:guid}/backlogs")]
public class BacklogsController : ApiController
{
    private readonly ISender _sender;

    public BacklogsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get the backlog for a team.
    /// </summary>
    /// <param name="teamId">The ID of the team to get the backlog for.</param>
    /// <param name="request">The details of the backlog to get.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<BacklogResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWorkItem(
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