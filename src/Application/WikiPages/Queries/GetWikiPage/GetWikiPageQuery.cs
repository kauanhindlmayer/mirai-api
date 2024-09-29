using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Queries.GetWikiPage;

public record GetWikiPageQuery(Guid WikiPageId) : IRequest<ErrorOr<WikiPage>>;