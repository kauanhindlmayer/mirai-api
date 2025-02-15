using Domain.Retrospectives.Enums;

namespace Contracts.Retrospectives;

public sealed record CreateRetrospectiveRequest
{
    /// <summary>
    /// The title of the retrospective.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// The maximum number of votes per user.
    /// </summary>
    public int? MaxVotesPerUser { get; init; }

    /// <summary>
    /// The retrospective template to use.
    /// </summary>
    public RetrospectiveTemplate? Template { get; init; }
}