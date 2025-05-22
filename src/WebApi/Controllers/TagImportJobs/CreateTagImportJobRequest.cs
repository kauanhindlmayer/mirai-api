namespace WebApi.Controllers.TagImportJobs;

/// <summary>
/// Data transfer object for creating a tag import job.
/// </summary>
/// <param name="File">The file containing the tags to import.</param>
public sealed record CreateTagImportJobRequest(IFormFile File);