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
}
