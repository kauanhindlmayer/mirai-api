using ErrorOr;

namespace Domain.Tags;

public static class TagImportJobErrors
{
    public static readonly Error NotFound = Error.NotFound(
        code: "TagImportJob.NotFound",
        description: "Tag import job not found.");
}
