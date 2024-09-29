using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.DeleteWikiPage;

public record DeleteWikiPageCommand(Guid WikiPageId) : IRequest<ErrorOr<Success>>;