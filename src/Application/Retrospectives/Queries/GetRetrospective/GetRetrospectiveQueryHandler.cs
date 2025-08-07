using Application.Abstractions;
using Application.Retrospectives.Queries.Common;
using Domain.Retrospectives;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Retrospectives.Queries.GetRetrospective;

internal sealed class GetRetrospectiveQueryHandler
    : IRequestHandler<GetRetrospectiveQuery, ErrorOr<RetrospectiveResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetRetrospectiveQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<RetrospectiveResponse>> Handle(
        GetRetrospectiveQuery query,
        CancellationToken cancellationToken)
    {
        var retrospective = await _context.Retrospectives
            .AsNoTracking()
            .Where(r => r.Id == query.RetrospectiveId)
            .Select(RetrospectiveQueries.ProjectToDto())
            .FirstOrDefaultAsync(cancellationToken);

        if (retrospective is null)
        {
            return RetrospectiveErrors.NotFound;
        }

        return retrospective;
    }
}