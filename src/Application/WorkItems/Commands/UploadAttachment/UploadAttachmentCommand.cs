using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.UploadAttachment;

public sealed record UploadAttachmentCommand(
    Guid WorkItemId,
    string FileName,
    string ContentType,
    long FileSizeBytes,
    Stream FileStream) : IRequest<ErrorOr<Guid>>;
