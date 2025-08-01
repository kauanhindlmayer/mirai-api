using System.Net.Mime;
using Application.Personas.Commands.CreatePersona;
using Application.Personas.Commands.DeletePersona;
using Application.Personas.Commands.UpdatePersona;
using Application.Personas.Queries.GetPersona;
using Application.Personas.Queries.ListPersonas;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Constants;

namespace Presentation.Controllers.Personas;

[ApiVersion(ApiVersions.V1)]
[Route("api/projects/{projectId:guid}/personas")]
[Produces(MediaTypeNames.Application.Json, CustomMediaTypeNames.Application.JsonV1)]
public sealed class PersonasController : ApiController
{
    private readonly ISender _sender;

    public PersonasController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Create a persona.
    /// </summary>
    /// <param name="projectId">The project's unique identifier.</param>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreatePersona(
        Guid projectId,
        [FromForm] CreatePersonaRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreatePersonaCommand(
            projectId,
            request.Name,
            request.Category,
            request.Description,
            request.File);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            personaId => CreatedAtAction(
                nameof(GetPersona),
                new { projectId, personaId },
                personaId),
            Problem);
    }

    /// <summary>
    /// Retrieve a persona.
    /// </summary>
    /// <param name="personaId">The persona's unique identifier.</param>
    [HttpGet("{personaId:guid}")]
    [ProducesResponseType(typeof(PersonaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonaResponse>> GetPersona(
        Guid personaId,
        CancellationToken cancellationToken)
    {
        var query = new GetPersonaQuery(personaId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// List all personas for a project.
    /// </summary>
    /// <param name="projectId">The project's unique identifier.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PersonaBriefResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PersonaBriefResponse>>> ListPersonas(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var query = new ListPersonasQuery(projectId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(Ok, Problem);
    }

    /// <summary>
    /// Update a persona.
    /// </summary>
    /// <param name="projectId">The project's unique identifier.</param>
    /// <param name="personaId">The persona's unique identifier.</param>
    [HttpPut("{personaId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePersona(
        Guid projectId,
        Guid personaId,
        [FromForm] UpdatePersonaRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdatePersonaCommand(
            projectId,
            personaId,
            request.Name,
            request.Description,
            request.File);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Delete a persona.
    /// </summary>
    /// <param name="projectId">The project's unique identifier.</param>
    /// <param name="personaId">The persona's unique identifier.</param>
    [HttpDelete("{personaId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePersona(
        Guid projectId,
        Guid personaId,
        CancellationToken cancellationToken)
    {
        var command = new DeletePersonaCommand(projectId, personaId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }
}