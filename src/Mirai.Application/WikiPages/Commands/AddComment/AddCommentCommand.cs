using ErrorOr;
using MediatR;
using Mirai.Domain.WikiPages;

namespace Mirai.Application.WikiPages.Commands.AddComment;

public record AddCommentCommand(Guid WikiPageId, string Content)
    : IRequest<ErrorOr<WikiPageComment>>;