using System.Net.Mime;
using Application.Authorization.Queries.ListRoles;
using Asp.Versioning;
using Domain.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Constants;

namespace Presentation.Controllers.Authorization;

[ApiVersion(ApiVersions.V1)]
[Route("api/roles")]
[Produces(MediaTypeNames.Application.Json, CustomMediaTypeNames.Application.JsonV1)]
public sealed class RolesController : ApiController
{
    private readonly ISender _sender;

    public RolesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// List the fixed catalog of system roles.
    /// </summary>
    /// <param name="scope">Optional scope filter (Organization, Project, or Team).</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<RoleResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RoleResponse>>> ListRoles(
        [FromQuery] RoleScope? scope,
        CancellationToken cancellationToken)
    {
        var query = new ListRolesQuery(scope);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }
}
