using Application.Organizations.Commands.CreateOrganization;
using Application.Organizations.Commands.DeleteOrganization;
using Application.Organizations.Commands.UpdateOrganization;
using Application.Organizations.Queries.GetOrganization;
using Application.Organizations.Queries.ListOrganizations;
using Asp.Versioning;
using Contracts.Organizations;
using Domain.Organizations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/organizations")]
public class OrganizationsController(ISender sender) : ApiController
{
    /// <summary>
    /// Create a new organization.
    /// </summary>
    /// <param name="request">The request to create a new organization.</param>
    [HttpPost]
    [ProducesResponseType(typeof(OrganizationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrganization(
        CreateOrganizationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateOrganizationCommand(
            request.Name,
            request.Description);

        var result = await sender.Send(command, cancellationToken);

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
    [HttpGet("{organizationId:guid}")]
    [ProducesResponseType(typeof(OrganizationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrganization(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var query = new GetOrganizationQuery(organizationId);

        var result = await sender.Send(query, cancellationToken);

        return result.Match(
            organization => Ok(ToDto(organization)),
            Problem);
    }

    /// <summary>
    /// List all organizations.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<OrganizationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListOrganizations(CancellationToken cancellationToken)
    {
        var query = new ListOrganizationsQuery();

        var result = await sender.Send(query, cancellationToken);

        return result.Match(
            organizations => Ok(organizations.ConvertAll(ToDto)),
            Problem);
    }

    /// <summary>
    /// Update an organization.
    /// </summary>
    /// <param name="organizationId">The ID of the organization to update.</param>
    /// <param name="request">The request to update the organization.</param>
    [HttpPut("{organizationId:guid}")]
    [ProducesResponseType(typeof(OrganizationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOrganization(
        Guid organizationId,
        UpdateOrganizationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateOrganizationCommand(
            organizationId,
            request.Name,
            request.Description);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            organization => Ok(ToDto(organization)),
            Problem);
    }

    /// <summary>
    /// Delete an organization.
    /// </summary>
    /// <param name="organizationId">The ID of the organization to delete.</param>
    [HttpDelete("{organizationId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteOrganization(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteOrganizationCommand(organizationId);

        var result = await sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
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