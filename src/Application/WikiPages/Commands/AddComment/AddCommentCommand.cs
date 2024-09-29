using Domain.WikiPages;
using ErrorOr;
using MediatR;

namespace Application.WikiPages.Commands.AddComment;

public record AddCommentCommand(Guid WikiPageId, string Content)
    : IRequest<ErrorOr<WikiPageComment>>;