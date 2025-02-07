using Application.Dashboards.Queries.GetDashboard;
using Asp.Versioning;
using Contracts.Dashboards;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/teams/{teamId:guid}/dashboards")]
public class DashboardsController : ApiController
{
    private readonly ISender _sender;

    public DashboardsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Retrieve the dashboard data for a project.
    /// </summary>
    /// <remarks>
    /// Retrieves the dashboard data for the specified team. The dashboard data
    /// can be filtered by start and end date.
    /// </remarks>
    /// <param name="teamId">The team's unique identifier.</param>
    [HttpGet]
    [ProducesResponseType(typeof(DashboardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDashboard(
        [FromRoute] Guid teamId,
        [FromQuery] GetDashboardRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetDashboardQuery(
            teamId,
            request.StartDate,
            request.EndDate);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }
}