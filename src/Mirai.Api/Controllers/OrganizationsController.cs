using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mirai.Application.Organizations.Commands.CreateOrganization;
using Mirai.Application.Organizations.Commands.UpdateOrganization;
using Mirai.Application.Organizations.Queries.GetOrganization;
using Mirai.Application.Organizations.Queries.ListOrganizations;
using Mirai.Contracts.Organizations;
using Mirai.Domain.Organizations;

namespace Mirai.Api.Controllers;

[AllowAnonymous]
public class OrganizationsController(ISender _mediator) : ApiController
{
    /// <summary>
    /// Create a new organization.
    /// </summary>
    /// <param name="request">The request to create a new organization.</param>
    /// <returns>The newly created organization.</returns>
    [HttpPost(ApiEndpoints.Organizations.Create)]
    [ProducesResponseType(typeof(OrganizationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrganization(CreateOrganizationRequest request)
    {
        var command = new CreateOrganizationCommand(request.Name, request.Description);

        var result = await _mediator.Send(command);

        return result.Match(
            organization => CreatedAtAction(
                actionName: nameof(GetOrganization),
                routeValues: new { OrganizationId = organization.Id },
                value: ToDto(organization)),
            Problem);
    }

    /// <summary>
    /// Get an organization by its ID.
    /// </summary>
    /// <param name="organizationId">The ID of the organization to get.</param>
    /// <returns>The organization with the specified ID.</returns>
    [HttpGet(ApiEndpoints.Organizations.Get)]
    [ProducesResponseType(typeof(IEnumerable<OrganizationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrganization(Guid organizationId)
    {
        var query = new GetOrganizationQuery(organizationId);

        var result = await _mediator.Send(query);

        return result.Match(
            organization => Ok(ToDto(organization)),
            Problem);
    }

    /// <summary>
    /// List all organizations.
    /// </summary>
    /// <returns>All organizations.</returns>
    [HttpGet(ApiEndpoints.Organizations.List)]
    [ProducesResponseType(typeof(IEnumerable<OrganizationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListOrganizations()
    {
        var query = new ListOrganizationsQuery();

        var result = await _mediator.Send(query);

        return result.Match(
            organizations => Ok(organizations.ConvertAll(ToDto)),
            Problem);
    }

    /// <summary>
    /// Update an organization.
    /// </summary>
    /// <param name="organizationId">The ID of the organization to update.</param>
    /// <param name="request">The request to update the organization.</param>
    /// <returns>The updated organization.</returns>
    [HttpPut(ApiEndpoints.Organizations.Update)]
    [ProducesResponseType(typeof(OrganizationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOrganization(Guid organizationId, UpdateOrganizationRequest request)
    {
        var command = new UpdateOrganizationCommand(organizationId, request.Name, request.Description);

        var result = await _mediator.Send(command);

        return result.Match(
            organization => Ok(ToDto(organization)),
            Problem);
    }

    private static OrganizationResponse ToDto(Organization organization)
    {
        return new(
            organization.Id,
            organization.Name,
            organization.Description,
            organization.CreatedAt,
            organization.UpdatedAt);
    }
}