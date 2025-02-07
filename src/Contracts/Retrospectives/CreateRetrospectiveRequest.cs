namespace Contracts.Retrospectives;

public sealed record CreateRetrospectiveRequest
{
    /// <summary>
    /// The title of the retrospective.
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// The description of the retrospective.
    /// </summary>
    public string Description { get; init; } = string.Empty;
}