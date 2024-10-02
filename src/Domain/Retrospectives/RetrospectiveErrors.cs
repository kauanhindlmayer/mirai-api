using ErrorOr;

namespace Domain.Retrospectives;

public static class RetrospectiveErrors
{
    public static readonly Error RetrospectiveNotFound = Error.NotFound(
        code: "Retrospective.RetrospectiveNotFound",
        description: "Retrospective not found.");

    public static readonly Error RetrospectiveColumnNotFound = Error.NotFound(
        code: "Retrospective.RetrospectiveColumnNotFound",
        description: "Retrospective column not found.");

    public static readonly Error RetrospectiveItemNotFound = Error.NotFound(
        code: "Retrospective.RetrospectiveItemNotFound",
        description: "Retrospective item not found.");

    public static readonly Error RetrospectiveAlreadyExists = Error.Validation(
        code: "Retrospective.RetrospectiveAlreadyExists",
        description: "A retrospective with the same title already exists in this team.");

    public static readonly Error RetrospectiveColumnAlreadyExists = Error.Validation(
        code: "Retrospective.RetrospectiveColumnAlreadyExists",
        description: "A column with the same title already exists in this retrospective.");

    public static readonly Error RetrospectiveItemAlreadyExists = Error.Validation(
        code: "Retrospective.RetrospectiveItemAlreadyExists",
        description: "An item with the same description already exists in this column.");
}