using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.DeleteWikiPage;

public sealed record DeleteWikiPageCommand(Guid WikiPageId) : IRequest<ErrorOr<Success>>;