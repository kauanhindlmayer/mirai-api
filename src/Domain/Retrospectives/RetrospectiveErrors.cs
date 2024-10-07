using ErrorOr;

namespace Domain.Retrospectives;

public static class RetrospectiveErrors
{
    public static readonly Error NotFound = Error.NotFound(
        code: "Retrospective.NotFound",
        description: "Retrospective not found.");

    public static readonly Error ColumnNotFound = Error.NotFound(
        code: "Retrospective.ColumnNotFound",
        description: "Retrospective column not found.");

    public static readonly Error ItemNotFound = Error.NotFound(
        code: "Retrospective.ItemNotFound",
        description: "Retrospective item not found.");

    public static readonly Error AlreadyExists = Error.Validation(
        code: "Retrospective.AlreadyExists",
        description: "A retrospective with the same title already exists in this team.");

    public static readonly Error ColumnAlreadyExists = Error.Validation(
        code: "Retrospective.ColumnAlreadyExists",
        description: "A column with the same title already exists in this retrospective.");

    public static readonly Error ItemAlreadyExists = Error.Validation(
        code: "Retrospective.ItemAlreadyExists",
        description: "An item with the same description already exists in this column.");
}