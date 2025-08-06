using ErrorOr;

namespace Domain.Common;

public static class Errors
{
    public static Error InvalidSort(string? sort) => Error.Validation(
        code: "Sort.Invalid",
        description: $"The sort parameter '{sort}' is not valid. Please use a valid sort field.");

    public static readonly Error EmbeddingFailure = Error.Failure(
        code: "Nlp.EmbeddingFailure",
        description: "Unexpected error occurred during embedding generation.");

    public static readonly Error AnsweringFailure = Error.Failure(
        code: "Nlp.AnsweringFailure",
        description: "Unexpected error occurred during question answering.");
}
