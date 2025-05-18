namespace Application.TagImportJobs.Queries.GetTagImportJob;

public sealed class TagImportJobResponse
{
    public Guid Id { get; init; }
    public required string Status { get; init; }
    public required string FileName { get; init; }
    public int TotalRecords { get; init; }
    public int ProcessedRecords { get; init; }
    public int SuccessfulRecords { get; init; }
    public int FailedRecords { get; init; }
    public required List<string> Errors { get; init; } = [];
    public required DateTime CreatedAtUtc { get; init; }
    public DateTime? CompletedAtUtc { get; init; }
}