namespace Application.WorkItems.Queries.DownloadAttachment;

public sealed record AttachmentFileResponse(
    Stream Stream,
    string ContentType,
    string FileName);
