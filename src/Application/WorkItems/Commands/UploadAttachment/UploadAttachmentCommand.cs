using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.WorkItems.Commands.UploadAttachment;

public sealed record UploadAttachmentCommand(
    Guid WorkItemId,
    string FileName,
    string ContentType,
    long FileSizeBytes,
    Stream FileStream) : IAuthorizationRequest<ErrorOr<Guid>>
{
    public Permission RequiredPermission => Permission.ProjectManageWorkItems;
    public ResourceType ResourceType => ResourceType.WorkItem;
    public Guid ResourceId => WorkItemId;
}
