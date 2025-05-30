using Application.Common.Interfaces.Persistence;
using Application.Personas.Queries.Common;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Personas.Queries.ListPersonas;

internal sealed class ListPersonasQueryHandler
    : IRequestHandler<ListPersonasQuery, ErrorOr<IReadOnlyList<PersonaBriefResponse>>>
{
    private readonly IApplicationDbContext _context;

    public ListPersonasQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<IReadOnlyList<PersonaBriefResponse>>> Handle(
        ListPersonasQuery query,
        CancellationToken cancellationToken)
    {
        var personas = await _context.Personas
            .AsNoTracking()
            .Where(p => p.ProjectId == query.ProjectId)
            .Select(PersonaQueries.ProjectToBriefDto())
            .ToListAsync(cancellationToken);

        return personas;
    }
}