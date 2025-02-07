using Application.Organizations.Commands.CreateOrganization;
using Application.Organizations.Commands.DeleteOrganization;
using Application.Organizations.Commands.UpdateOrganization;
using Application.Organizations.Queries.GetOrganization;
using Application.Organizations.Queries.ListOrganizations;
using Asp.Versioning;
using Contracts.Organizations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/organizations")]
public class OrganizationsController : ApiController
{
    private readonly ISender _sender;

    public OrganizationsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Create a organization.
    /// </summary>
    /// <remarks>
    /// Creates a new organization object.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrganization(
        CreateOrganizationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateOrganizationCommand(
            request.Name,
            request.Description);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            organizationId => CreatedAtAction(
                nameof(GetOrganization),
                new { organizationId },
                organizationId),
            Problem);
    }

    /// <summary>
    /// Retrieve an organization.
    /// </summary>
    /// <remarks>
    /// Retrieves the organization with the specified unique identifier.
    /// </remarks>
    /// <param name="organizationId">The organization's unique identifier.</param>
    [HttpGet("{organizationId:guid}")]
    [ProducesResponseType(typeof(OrganizationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrganization(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var query = new GetOrganizationQuery(organizationId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// List all organizations.
    /// </summary>
    /// <remarks>
    /// Returns a list of your organizations. The organizations are returned
    /// sorted by name in ascending order.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<OrganizationBriefResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListOrganizations(CancellationToken cancellationToken)
    {
        var query = new ListOrganizationsQuery();

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Update an organization.
    /// </summary>
    /// <remarks>
    /// Updates the specified organization by setting the values of the
    /// parameters passed. Any parameters not provided will be left unchanged.
    /// </remarks>
    /// <param name="organizationId">The organization's unique identifier.</param>
    [HttpPut("{organizationId:guid}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
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

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => Ok(organizationId),
            Problem);
    }

    /// <summary>
    /// Delete an organization.
    /// </summary>
    /// <remarks>
    /// Deletes the specified organization. Deleting is only possible if the
    /// organization does not have any projects associated with it.
    /// </remarks>
    /// <param name="organizationId">The organization's unique identifier.</param>
    [HttpDelete("{organizationId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteOrganization(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteOrganizationCommand(organizationId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }
}