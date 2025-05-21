using Domain.Retrospectives.Enums;

namespace WebApi.Controllers.Retrospectives;

/// <summary>
/// Data transfer object for creating a retrospective board.
/// </summary>
/// <param name="Title">The title of the retrospective.</param>
/// <param name="MaxVotesPerUser">The maximum number of votes per user.</param>
/// <param name="Template">The retrospective template to use.</param>
public sealed record CreateRetrospectiveRequest(
    string Title,
    int? MaxVotesPerUser,
    RetrospectiveTemplate? Template);