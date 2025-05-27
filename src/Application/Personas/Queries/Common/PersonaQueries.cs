using System.Linq.Expressions;
using Application.Personas.Queries.GetPersona;
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
            AvatarUrl = p.AvatarUrl,
            ProjectId = p.ProjectId,
            CreatedAtUtc = p.CreatedAtUtc,
            UpdatedAtUtc = p.UpdatedAtUtc,
        };
    }
}