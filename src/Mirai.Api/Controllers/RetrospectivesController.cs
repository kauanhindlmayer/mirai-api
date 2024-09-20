using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.Retrospectives.Commands.CreateRetrospective;
using Mirai.Application.Retrospectives.Queries.GetRetrospective;
using Mirai.Contracts.Retrospectives;
using Mirai.Domain.Retrospectives;

namespace Mirai.Api.Controllers;

public class RetrospectivesController(ISender _mediator) : ApiController
{
    /// <summary>
    /// Create a new retrospective session.
    /// </summary>
    /// <param name="request">The details of the retrospective session to create.</param>
    [HttpPost(ApiEndpoints.Retrospectives.Create)]
    [ProducesResponseType(typeof(RetrospectiveResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRetrospective(CreateRetrospectiveRequest request)
    {
        var command = new CreateRetrospectiveCommand(
            Title: request.Title,
            Description: request.Description,
            ProjectId: request.ProjectId);

        var result = await _mediator.Send(command);

        return result.Match(
            retrospective => CreatedAtAction(
                actionName: nameof(GetRetrospective),
                routeValues: new { RetrospectiveId = retrospective.Id },
                value: ToDto(retrospective)),
            Problem);
    }

    /// <summary>
    /// Get a retrospective session by ID.
    /// </summary>
    /// <param name="retrospectiveId">The retrospective session ID.</param>
    [HttpGet(ApiEndpoints.Retrospectives.Get)]
    [ProducesResponseType(typeof(RetrospectiveResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRetrospective(Guid retrospectiveId)
    {
        var query = new GetRetrospectiveQuery(retrospectiveId);

        var result = await _mediator.Send(query);

        return result.Match(
            retrospective => Ok(ToDto(retrospective)),
            Problem);
    }

    private static RetrospectiveResponse ToDto(Retrospective retrospective)
    {
        return new RetrospectiveResponse(
            Id: retrospective.Id,
            Name: retrospective.Title,
            Description: retrospective.Description,
            Columns: [],
            CreatedAt: retrospective.CreatedAt,
            UpdatedAt: retrospective.UpdatedAt);
    }
}