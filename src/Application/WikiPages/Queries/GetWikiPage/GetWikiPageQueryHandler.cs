using Application.Abstractions;
using Application.Abstractions.Authentication;
using Domain.WikiPages;
using Domain.WikiPages.Events;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.WikiPages.Queries.GetWikiPage;

internal sealed class GetWikiPageQueryHandler
    : IRequestHandler<GetWikiPageQuery, ErrorOr<WikiPageResponse>>
{
    private readonly IUserContext _userContext;
    private readonly IApplicationDbContext _context;
    private readonly IPublisher _publisher;

    public GetWikiPageQueryHandler(
        IUserContext userContext,
        IApplicationDbContext context,
        IPublisher publisher)
    {
        _userContext = userContext;
        _context = context;
        _publisher = publisher;
    }

    public async Task<ErrorOr<WikiPageResponse>> Handle(
        GetWikiPageQuery query,
        CancellationToken cancellationToken)
    {
        var wikiPage = await _context.WikiPages
            .AsNoTracking()
            .Where(wp => wp.Id == query.WikiPageId)
            .Select(WikiPageQueries.ProjectToDto())
            .FirstOrDefaultAsync(cancellationToken);

        if (wikiPage is null)
        {
            return WikiPageErrors.NotFound;
        }

        await _publisher.Publish(
            new WikiPageViewedDomainEvent(wikiPage.Id, _userContext.UserId),
            cancellationToken);

        return wikiPage;
    }
}