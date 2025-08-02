namespace Infrastructure.Services.Nlp;

/// <summary>
/// Response containing the answer to a question.
/// </summary>
/// <param name="Answer">The answer to the question based on the provided context.</param>
public sealed record AnswerQuestionResponse(string Answer);
