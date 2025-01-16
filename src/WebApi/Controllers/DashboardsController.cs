using Application.Dashboards.Queries.GetDashboard;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/projects/{projectId:guid}/dashboards")]
public class DashboardsController(ISender sender) : ApiController
{
    /// <summary>
    /// Retrieves the dashboard data for a given project.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <param name="startDate">The optional start date for filtering.</param>
    /// <param name="endDate">The optional end date for filtering.</param>
    [HttpGet]
    [ProducesResponseType(typeof(DashboardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDashboard(
        [FromRoute] Guid projectId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        CancellationToken cancellationToken)
    {
        var query = new GetDashboardQuery(
            projectId,
            startDate,
            endDate);

        var result = await sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }
}