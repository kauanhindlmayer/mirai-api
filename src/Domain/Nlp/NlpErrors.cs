using ErrorOr;

namespace Domain.Nlp;

public static class NlpErrors
{
    public static readonly Error EmbeddingFailure = Error.Failure(
        code: "Nlp.EmbeddingFailure",
        description: "Unexpected error occurred during embedding generation.");

    public static readonly Error AnsweringFailure = Error.Failure(
        code: "Nlp.AnsweringFailure",
        description: "Unexpected error occurred during question answering.");
}
