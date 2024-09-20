using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.Retrospectives.Commands.CreateRetrospective;
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
                actionName: "GetRetrospective", // nameof(GetRetrospective),
                routeValues: new { RetrospectiveId = retrospective.Id },
                value: ToDto(retrospective)),
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