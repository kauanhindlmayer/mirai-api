using System.Linq.Expressions;
using Application.Personas.Queries.GetPersona;
using Application.Personas.Queries.ListPersonas;
using Domain.Personas;

namespace Application.Personas.Queries.Common;

internal static class PersonaQueries
{
    public static Expression<Func<Persona, PersonaResponse>> ProjectToDto()
    {
        return p => new PersonaResponse
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            ImageUrl = p.ImageUrl,
            ProjectId = p.ProjectId,
            CreatedAtUtc = p.CreatedAtUtc,
            UpdatedAtUtc = p.UpdatedAtUtc,
        };
    }

    public static Expression<Func<Persona, PersonaBriefResponse>> ProjectToBriefDto()
    {
        return p => new PersonaBriefResponse
        {
            Id = p.Id,
            Name = p.Name,
            ImageUrl = p.ImageUrl,
        };
    }
}