using ErrorOr;
using MediatR;
using Mirai.Domain.WikiPages;

namespace Mirai.Application.WikiPages.Commands.CreateWikiPage;

public record CreateWikiPageCommand(Guid ProjectId, string Title, string Content)
    : IRequest<ErrorOr<WikiPage>>;