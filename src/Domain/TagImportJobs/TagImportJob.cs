namespace Domain.TagImportJobs;

public sealed class TagImportJob
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ProjectId { get; private set; }
    public TagImportStatus Status { get; private set; }
    public string FileName { get; private set; } = null!;
    public byte[] FileContent { get; private set; } = [];
    public int TotalRecords { get; private set; }
    public int ProcessedRecords { get; private set; }
    public int SuccessfulRecords { get; private set; }
    public int FailedRecords { get; private set; }
    public List<string> Errors { get; private set; } = [];
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }

    public TagImportJob(
        Guid projectId,
        string fileName,
        byte[] fileContent)
    {
        ProjectId = projectId;
        FileName = fileName;
        FileContent = fileContent;
        Status = TagImportStatus.Pending;
        CreatedAtUtc = DateTime.UtcNow;
    }

    private TagImportJob()
    {
    }

    public void SetTotalRecords(int totalRecords)
    {
        TotalRecords = totalRecords;
    }

    public void IncrementProcessedRecords()
    {
        ProcessedRecords++;
    }

    public void IncrementSuccessfulRecords()
    {
        SuccessfulRecords++;
    }

    public void IncrementFailedRecords()
    {
        FailedRecords++;
    }

    public void StartProcessing()
    {
        Status = TagImportStatus.Processing;
    }

    public void FailProcessing(string error)
    {
        Status = TagImportStatus.Failed;
        Errors.Add(error);
        CompletedAtUtc = DateTime.UtcNow;
    }

    public void CompleteProcessing()
    {
        Status = TagImportStatus.Completed;
        CompletedAtUtc = DateTime.UtcNow;
    }

    public void AddError(string error)
    {
        Errors.Add(error);
    }
}