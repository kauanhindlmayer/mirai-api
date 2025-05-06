using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Application.WikiPages.Common;
using Domain.WikiPages;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.WikiPages.Queries.GetWikiPage;

internal sealed class GetWikiPageQueryHandler : IRequestHandler<GetWikiPageQuery, ErrorOr<WikiPageResponse>>
{
    private readonly IWikiPagesRepository _wikiPagesRepository;
    private readonly IUserContext _userContext;
    private readonly IApplicationDbContext _context;

    public GetWikiPageQueryHandler(
        IWikiPagesRepository wikiPagesRepository,
        IUserContext userContext,
        IApplicationDbContext context)
    {
        _wikiPagesRepository = wikiPagesRepository;
        _userContext = userContext;
        _context = context;
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

        await _wikiPagesRepository.LogViewAsync(
            wikiPage.Id,
            _userContext.UserId,
            cancellationToken);

        return wikiPage;
    }
}