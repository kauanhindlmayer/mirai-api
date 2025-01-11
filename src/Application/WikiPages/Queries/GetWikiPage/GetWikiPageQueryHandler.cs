using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
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
            .Select(wp => new WikiPageResponse
            {
                Id = wp.Id,
                ProjectId = wp.ProjectId,
                Title = wp.Title,
                Content = wp.Content,
                Author = new AuthorResponse
                {
                    Name = wp.Author.FullName,
                    ImageUrl = wp.Author.ImageUrl,
                },
                Comments = wp.Comments.Select(comment => new WikiPageCommentResponse
                {
                    Id = comment.Id,
                    Author = new AuthorResponse
                    {
                        Name = comment.Author.FullName,
                        ImageUrl = comment.Author.ImageUrl,
                    },
                    Content = comment.Content,
                    CreatedAt = comment.CreatedAt,
                    UpdatedAt = comment.UpdatedAt,
                }),
                CreatedAt = wp.CreatedAt,
                UpdatedAt = wp.UpdatedAt,
            })
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