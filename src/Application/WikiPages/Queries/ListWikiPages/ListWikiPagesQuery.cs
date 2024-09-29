using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Queries.ListWikiPages;

public record ListWikiPagesQuery(Guid ProjectId) : IRequest<ErrorOr<List<WikiPage>>>;