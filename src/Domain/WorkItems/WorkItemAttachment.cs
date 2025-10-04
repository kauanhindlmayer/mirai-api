using Domain.Shared;

namespace Domain.WorkItems;

public sealed class WorkItemAttachment : Entity
{
    public Guid WorkItemId { get; private set; }
    public WorkItem WorkItem { get; private set; } = null!;
    public string FileName { get; private set; } = null!;
    public Guid BlobId { get; private set; }
    public string ContentType { get; private set; } = null!;
    public long FileSizeBytes { get; private set; }
    public Guid AuthorId { get; private set; }

    public WorkItemAttachment(
        Guid workItemId,
        string fileName,
        Guid blobId,
        string contentType,
        long fileSizeBytes,
        Guid authorId)
    {
        WorkItemId = workItemId;
        FileName = fileName;
        BlobId = blobId;
        ContentType = contentType;
        FileSizeBytes = fileSizeBytes;
        AuthorId = authorId;
    }

    private WorkItemAttachment()
    {
    }
}
