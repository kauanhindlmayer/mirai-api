namespace Presentation.Controllers.TagImportJobs;

/// <summary>
/// Request to create a new tag import job.
/// </summary>
/// <param name="File">The file containing the tags to import.</param>
public sealed record CreateTagImportJobRequest(IFormFile File);