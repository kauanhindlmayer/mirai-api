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

    public static readonly Error RetrospectiveColumnAlreadyExists = Error.Validation(
        code: "Retrospective.RetrospectiveColumnAlreadyExists",
        description: "A column with the same title already exists in this retrospective.");
}