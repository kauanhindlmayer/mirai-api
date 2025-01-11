using ErrorOr;
using MediatR;

namespace Application.WikiPages.Queries.ListWikiPages;

public sealed record ListWikiPagesQuery(Guid ProjectId) : IRequest<ErrorOr<List<WikiPageBriefResponse>>>;