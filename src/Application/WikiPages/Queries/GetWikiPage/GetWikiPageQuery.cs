using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Queries.GetWikiPage;

public sealed record GetWikiPageQuery(Guid WikiPageId) : IRequest<ErrorOr<WikiPageResponse>>;