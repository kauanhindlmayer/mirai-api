using Application.Abstractions.Authentication;
using Application.Abstractions.Storage;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.UploadAttachment;

internal sealed class UploadAttachmentCommandHandler
    : IRequestHandler<UploadAttachmentCommand, ErrorOr<Guid>>
{
    private readonly IWorkItemRepository _workItemRepository;
    private readonly IBlobService _blobService;
    private readonly IUserContext _userContext;

    public UploadAttachmentCommandHandler(
        IWorkItemRepository workItemRepository,
        IBlobService blobService,
        IUserContext userContext)
    {
        _workItemRepository = workItemRepository;
        _blobService = blobService;
        _userContext = userContext;
    }

    public async Task<ErrorOr<Guid>> Handle(
        UploadAttachmentCommand command,
        CancellationToken cancellationToken)
    {
        var workItem = await _workItemRepository.GetByIdAsync(
            command.WorkItemId,
            cancellationToken);

        if (workItem is null)
        {
            return WorkItemErrors.NotFound;
        }

        var uploadResponse = await _blobService.UploadAsync(
            command.FileStream,
            command.ContentType,
            cancellationToken);

        var attachment = new WorkItemAttachment(
            workItem.Id,
            command.FileName,
            uploadResponse.FileId,
            command.ContentType,
            command.FileSizeBytes,
            _userContext.UserId);

        workItem.AddAttachment(attachment);
        _workItemRepository.Update(workItem);

        return attachment.Id;
    }
}
