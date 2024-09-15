using ErrorOr;
using MediatR;

namespace Mirai.Application.WikiPages.Commands.DeleteWikiPage;

public record DeleteWikiPageCommand(Guid WikiPageId) : IRequest<ErrorOr<Success>>;