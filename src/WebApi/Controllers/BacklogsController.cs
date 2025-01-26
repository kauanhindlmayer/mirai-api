using Application.Backlogs.Queries.GetBacklog;
using Asp.Versioning;
using Contracts.Backlogs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using DomainBacklogLevel = Domain.Backlogs.BacklogLevel;

namespace WebApi.Controllers;

[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/teams/{teamId:guid}/backlogs")]
public class BacklogsController(ISender sender) : ApiController
{
    /// <summary>
    /// Get the backlog for a team.
    /// </summary>
    /// <param name="teamId">The ID of the team to get the backlog for.</param>
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

        var result = await sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }
}