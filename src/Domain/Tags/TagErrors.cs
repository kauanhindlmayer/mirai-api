using ErrorOr;

namespace Domain.Tags;

public static class TagErrors
{
    public static readonly Error TagWithSameNameAlreadyExists = Error.Conflict(
        code: "Tag.TagWithSameNameAlreadyExists",
        description: "Tag with the same name already exists.");

    public static readonly Error TagNotFound = Error.NotFound(
        code: "Tag.TagNotFound",
        description: "Tag not found.");
}
