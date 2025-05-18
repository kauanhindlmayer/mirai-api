using ErrorOr;

namespace Domain.Tags;

public static class TagErrors
{
    public static readonly Error NotFound = Error.NotFound(
        code: "Tag.NotFound",
        description: "Tag not found.");

    public static readonly Error AlreadyExists = Error.Conflict(
        code: "Tag.AlreadyExists",
        description: "Tag with the same name already exists.");

    public static readonly Error TagHasWorkItems = Error.Conflict(
        code: "Tag.TagHasWorkItems",
        description: "The tag has work items associated with it.");

    public static readonly Error CannotMergeIntoItself = Error.Validation(
        code: "Tag.CannotMergeIntoItself",
        description: "Cannot merge a tag into itself.");

    public static readonly Error TargetTagNotFound = Error.NotFound(
        code: "Tag.TargetNotFound",
        description: "Target tag not found.");

    public static Error SourceTagNotFound(Guid sourceTagId) => Error.NotFound(
        code: "Tag.SourceNotFound",
        description: $"Source tag '{sourceTagId}' not found.");

    public static readonly Error SourceAndTargetTagsMustBelongToSameProject = Error.Validation(
        code: "Tag.SourceAndTargetTagsMustBelongToSameProject",
        description: "Source and target tags must belong to the same project.");
}
