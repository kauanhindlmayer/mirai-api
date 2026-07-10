using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.WorkItems.Queries.DownloadAttachment;

public sealed record DownloadAttachmentQuery(
    Guid WorkItemId,
    Guid AttachmentId) : IAuthorizationRequest<ErrorOr<AttachmentFileResponse>>
{
    public Permission RequiredPermission => Permission.ProjectView;
    public ResourceType ResourceType => ResourceType.WorkItem;
    public Guid ResourceId => WorkItemId;
}
