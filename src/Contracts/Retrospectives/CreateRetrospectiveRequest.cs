using Domain.Retrospectives.Enums;

namespace Contracts.Retrospectives;

public sealed record CreateRetrospectiveRequest
{
    /// <summary>
    /// The title of the retrospective.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// The description of the retrospective.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// The retrospective template to use.
    /// </summary>
    public RetrospectiveTemplate? Template { get; init; }
}