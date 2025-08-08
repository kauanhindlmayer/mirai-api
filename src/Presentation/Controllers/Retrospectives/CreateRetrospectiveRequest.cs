using Domain.Retrospectives.Enums;

namespace Presentation.Controllers.Retrospectives;

/// <summary>
/// Request to create a new retrospective.
/// </summary>
/// <param name="Title">The title of the retrospective.</param>
/// <param name="MaxVotesPerUser">The maximum number of votes per user.</param>
/// <param name="Template">The retrospective template to use.</param>
public sealed record CreateRetrospectiveRequest(
    string Title,
    int? MaxVotesPerUser,
    RetrospectiveTemplate? Template);