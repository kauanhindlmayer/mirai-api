namespace Presentation.Controllers.Projects;

/// <summary>
/// A pull request found while searching a project's connected GitHub repository.
/// </summary>
/// <param name="Id">The pull request's immutable GitHub numeric id.</param>
/// <param name="Number">The pull request's display number.</param>
/// <param name="Title">The pull request's title.</param>
/// <param name="HtmlUrl">The pull request's GitHub URL.</param>
/// <param name="IsOpen">Whether the pull request is currently open.</param>
/// <param name="IsMerged">Whether the pull request has been merged.</param>
/// <param name="AuthorLogin">The pull request author's GitHub login.</param>
public sealed record GitHubPullRequestResponse(
    long Id,
    int Number,
    string Title,
    string HtmlUrl,
    bool IsOpen,
    bool IsMerged,
    string? AuthorLogin);
