using ErrorOr;

namespace Mirai.Domain.Retrospectives;

public static class RetrospectiveErrors
{
    public static readonly Error RetrospectiveNotFound = Error.NotFound(
        code: "Retrospective.RetrospectiveNotFound",
        description: "Retrospective not found.");

    public static readonly Error RetrospectiveColumnNotFound = Error.NotFound(
        code: "Retrospective.RetrospectiveColumnNotFound",
        description: "Retrospective column not found.");
}