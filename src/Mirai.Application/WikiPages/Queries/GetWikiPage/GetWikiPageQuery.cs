using ErrorOr;
using MediatR;
using Mirai.Domain.WikiPages;

namespace Mirai.Application.WikiPages.Queries.GetWikiPage;

public record GetWikiPageQuery(Guid WikiPageId) : IRequest<ErrorOr<WikiPage>>;