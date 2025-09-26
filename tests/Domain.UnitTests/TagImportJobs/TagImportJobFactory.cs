using Domain.TagImportJobs;

namespace Domain.UnitTests.TagImportJobs;

internal static class TagImportJobFactory
{
    public const string FileName = "testFile.txt";
    public static readonly Guid ProjectId = Guid.NewGuid();
    public static readonly byte[] FileContent = [];

    public static TagImportJob Create(
        Guid? projectId = null,
        string? fileName = null,
        byte[]? fileContent = null)
    {
        return new TagImportJob(
            projectId ?? ProjectId,
            fileName ?? FileName,
            fileContent ?? FileContent);
    }
}