using Microsoft.AspNetCore.Http;

namespace Contracts.Tags;

/// <summary>
/// Data transfer object for creating a tag import job.
/// </summary>
/// <param name="File">The file containing the tags to import.</param>
public sealed record CreateTagImportJobRequest(IFormFile File);