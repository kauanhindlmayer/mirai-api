using Application.Sprints.Commands.CreateSprint;
using Asp.Versioning;
using Contracts.Sprints;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/teams/{teamId:guid}/sprints")]
public class SprintsController(ISender sender) : ApiController
{
    /// <summary>
    /// Create a sprint in a team.
    /// </summary>
    /// <param name="teamId">The team ID.</param>
    /// <param name="request">The sprint data.</param>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateSprint(
        Guid teamId,
        CreateSprintRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateSprintCommand(
            teamId,
            request.Name,
            request.StartDate,
            request.EndDate);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            sprintId => Ok(sprintId),
            Problem);
    }
}