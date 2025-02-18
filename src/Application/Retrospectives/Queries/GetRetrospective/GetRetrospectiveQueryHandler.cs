using Application.Common.Interfaces.Persistence;
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
            .Select(r => new RetrospectiveResponse
            {
                Id = r.Id,
                Title = r.Title,
                MaxVotesPerUser = r.MaxVotesPerUser,
                Columns = r.Columns.Select(c => new RetrospectiveColumnResponse
                {
                    Id = c.Id,
                    Title = c.Title,
                    Position = c.Position,
                    Items = c.Items.Select(i => new RetrospectiveItemResponse
                    {
                        Id = i.Id,
                        Content = i.Content,
                        Position = i.Position,
                        AuthorId = i.AuthorId,
                        Votes = i.Votes,
                        CreatedAt = i.CreatedAt,
                    }),
                }),
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (retrospective is null)
        {
            return RetrospectiveErrors.NotFound;
        }

        return retrospective;
    }
}