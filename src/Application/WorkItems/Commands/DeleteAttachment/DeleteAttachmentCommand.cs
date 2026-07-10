using Application.Abstractions.Authorization;
using Domain.Authorization;
using ErrorOr;

namespace Application.WorkItems.Commands.DeleteAttachment;

public sealed record DeleteAttachmentCommand(
    Guid WorkItemId,
    Guid AttachmentId) : IAuthorizationRequest<ErrorOr<Success>>
{
    public Permission RequiredPermission => Permission.ProjectManageWorkItems;
    public ResourceType ResourceType => ResourceType.WorkItem;
    public Guid ResourceId => WorkItemId;
}
