using Application.Abstractions;
using Application.Abstractions.Storage;
using Domain.WorkItems;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.WorkItems.Queries.DownloadAttachment;

internal sealed class DownloadAttachmentQueryHandler
    : IRequestHandler<DownloadAttachmentQuery, ErrorOr<AttachmentFileResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IBlobService _blobService;

    public DownloadAttachmentQueryHandler(
        IApplicationDbContext context,
        IBlobService blobService)
    {
        _context = context;
        _blobService = blobService;
    }

    public async Task<ErrorOr<AttachmentFileResponse>> Handle(
        DownloadAttachmentQuery query,
        CancellationToken cancellationToken)
    {
        var attachment = await _context.WorkItems
            .AsNoTracking()
            .Where(wi => wi.Id == query.WorkItemId)
            .SelectMany(wi => wi.Attachments)
            .FirstOrDefaultAsync(a => a.Id == query.AttachmentId, cancellationToken);

        if (attachment is null)
        {
            return WorkItemErrors.AttachmentNotFound;
        }

        var fileResponse = await _blobService.DownloadAsync(
            attachment.BlobId,
            cancellationToken);

        return new AttachmentFileResponse(
            fileResponse.Stream,
            attachment.ContentType,
            attachment.FileName);
    }
}
