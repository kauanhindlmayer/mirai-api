using ErrorOr;
using MediatR;

namespace Application.WorkItems.Queries.DownloadAttachment;

public sealed record DownloadAttachmentQuery(
    Guid WorkItemId,
    Guid AttachmentId) : IRequest<ErrorOr<AttachmentFileResponse>>;
