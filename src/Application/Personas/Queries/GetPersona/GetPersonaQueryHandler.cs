using Application.Abstractions;
using Domain.Personas;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Personas.Queries.GetPersona;

internal sealed class GetPersonaQueryHandler
    : IRequestHandler<GetPersonaQuery, ErrorOr<PersonaResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetPersonaQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<PersonaResponse>> Handle(
        GetPersonaQuery query,
        CancellationToken cancellationToken)
    {
        var persona = await _context.Personas
            .AsNoTracking()
            .Where(p => p.Id == query.PersonaId)
            .Select(PersonaQueries.ProjectToDto())
            .FirstOrDefaultAsync(cancellationToken);

        if (persona is null)
        {
            return PersonaErrors.NotFound;
        }

        return persona;
    }
}