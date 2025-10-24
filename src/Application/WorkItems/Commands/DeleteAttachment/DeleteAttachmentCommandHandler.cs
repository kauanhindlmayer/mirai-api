using Application.Abstractions.Storage;
using Domain.WorkItems;
using ErrorOr;
using MediatR;

namespace Application.WorkItems.Commands.DeleteAttachment;

internal sealed class DeleteAttachmentCommandHandler
    : IRequestHandler<DeleteAttachmentCommand, ErrorOr<Success>>
{
    private readonly IWorkItemRepository _workItemRepository;
    private readonly IBlobService _blobService;

    public DeleteAttachmentCommandHandler(
        IWorkItemRepository workItemRepository,
        IBlobService blobService)
    {
        _workItemRepository = workItemRepository;
        _blobService = blobService;
    }

    public async Task<ErrorOr<Success>> Handle(
        DeleteAttachmentCommand command,
        CancellationToken cancellationToken)
    {
        var workItem = await _workItemRepository.GetByIdAsync(
            command.WorkItemId,
            cancellationToken);

        if (workItem is null)
        {
            return WorkItemErrors.NotFound;
        }

        var result = workItem.RemoveAttachment(command.AttachmentId);
        if (result.IsError)
        {
            return result.Errors;
        }

        await _blobService.DeleteAsync(result.Value.BlobId, cancellationToken);

        _workItemRepository.Update(workItem);

        return Result.Success;
    }
}
