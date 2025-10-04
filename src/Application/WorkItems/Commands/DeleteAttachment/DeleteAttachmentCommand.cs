using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.DeleteAttachment;

public sealed record DeleteAttachmentCommand(
    Guid WorkItemId,
    Guid AttachmentId) : IRequest<ErrorOr<Success>>;
