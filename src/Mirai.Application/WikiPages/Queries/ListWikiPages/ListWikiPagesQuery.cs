using ErrorOr;
using MediatR;
using Mirai.Domain.WikiPages;

namespace Mirai.Application.WikiPages.Queries.ListWikiPages;

public record ListWikiPagesQuery(Guid ProjectId) : IRequest<ErrorOr<List<WikiPage>>>;