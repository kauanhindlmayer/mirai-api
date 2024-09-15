using ErrorOr;
using MediatR;
using Mirai.Domain.WikiPages;

namespace Mirai.Application.WikiPages.Commands.UpdateWikiPage;

public record UpdateWikiPageCommand(Guid WikiPageId, string Title, string Content)
    : IRequest<ErrorOr<WikiPage>>;