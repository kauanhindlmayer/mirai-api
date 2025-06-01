using System.Net.Mime;
using Application.Common;
using Application.Organizations.Commands.CreateOrganization;
using Application.Organizations.Commands.DeleteOrganization;
using Application.Organizations.Commands.UpdateOrganization;
using Application.Organizations.Queries.GetOrganization;
using Application.Organizations.Queries.ListOrganizations;
using Asp.Versioning;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Constants;

namespace Presentation.Controllers.Organizations;

[ApiVersion(ApiVersions.V1)]
[Route("api/organizations")]
[Produces(
    MediaTypeNames.Application.Json,
    CustomMediaTypeNames.Application.JsonV1,
    CustomMediaTypeNames.Application.HateoasJson,
    CustomMediaTypeNames.Application.HateoasJsonV1)]
public sealed class OrganizationsController : ApiController
{
    private readonly ISender _sender;
    private readonly LinkService _linkService;

    public OrganizationsController(
        ISender sender,
        LinkService linkService)
    {
        _sender = sender;
        _linkService = linkService;
    }

    /// <summary>
    /// Create an organization.
    /// </summary>
    /// <returns>The unique identifier of the created organization.</returns>
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
    /// <param name="organizationId">The organization's unique identifier.</param>
    [HttpGet("{organizationId:guid}")]
    [ProducesResponseType(typeof(OrganizationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrganizationResponse>> GetOrganization(
        Guid organizationId,
        [FromHeader] AcceptHeaderRequest acceptHeader,
        CancellationToken cancellationToken)
    {
        var query = new GetOrganizationQuery(organizationId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(
            organization =>
            {
                if (acceptHeader.IncludeLinks)
                {
                    organization.Links = CreateLinksForOrganization(organization.Id);
                }

                return Ok(organization);
            },
            Problem);
    }

    /// <summary>
    /// List all organizations.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<OrganizationBriefResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<OrganizationBriefResponse>>> ListOrganizations(
        [FromHeader] AcceptHeaderRequest acceptHeader,
        CancellationToken cancellationToken)
    {
        var query = new ListOrganizationsQuery();

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(
            organizations =>
            {
                if (acceptHeader.IncludeLinks)
                {
                    foreach (var organization in organizations)
                    {
                        organization.Links = CreateLinksForOrganization(organization.Id);
                    }
                }

                return Ok(organizations);
            },
            Problem);
    }

    /// <summary>
    /// Update an organization.
    /// </summary>
    /// <param name="organizationId">The organization's unique identifier.</param>
    /// <returns>The unique identifier of the updated organization.</returns>
    [HttpPut("{organizationId:guid}")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateOrganization(
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
    /// <param name="organizationId">The organization's unique identifier.</param>
    [HttpDelete("{organizationId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteOrganization(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteOrganizationCommand(organizationId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    private List<LinkResponse> CreateLinksForOrganization(Guid id)
    {
        var routeValues = new { organizationId = id };

        List<LinkResponse> links =
        [
            _linkService.Create(nameof(GetOrganization), "self", HttpMethods.Get, routeValues),
            _linkService.Create(nameof(UpdateOrganization), "update", HttpMethods.Put, routeValues),
            _linkService.Create(nameof(DeleteOrganization), "delete", HttpMethods.Delete, routeValues)
        ];

        return links;
    }
}
