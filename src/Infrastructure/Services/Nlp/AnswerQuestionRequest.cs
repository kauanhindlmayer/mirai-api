namespace Infrastructure.Services.Nlp;

/// <summary>
/// Request to answer a question based on a given context.
/// </summary>
/// <param name="Question">The question to be answered.</param>
/// <param name="Context">The context in which the question is asked.</param>
public sealed record AnswerQuestionRequest(
    string Question,
    string Context);