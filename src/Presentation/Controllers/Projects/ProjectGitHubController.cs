using System.Net.Mime;
using Application.Abstractions.GitHub;
using Application.Projects.Commands.ConnectGitHubRepository;
using Application.Projects.Commands.DisconnectGitHubRepository;
using Application.Projects.Queries.GetGitHubInstallationRepositories;
using Application.Projects.Queries.GetGitHubInstallUrl;
using Application.Projects.Queries.SearchGitHubPullRequests;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Constants;

namespace Presentation.Controllers.Projects;

/// <summary>
/// Manages a project's connection to a GitHub repository (kept separate from
/// <see cref="ProjectsController"/> to keep that controller's file size in check).
/// </summary>
[ApiVersion(ApiVersions.V1)]
[Route("api/organizations/{organizationId:guid}/projects/{projectId:guid}/github")]
[Produces(MediaTypeNames.Application.Json, CustomMediaTypeNames.Application.JsonV1)]
public sealed class ProjectGitHubController : ApiController
{
    private readonly ISender _sender;

    public ProjectGitHubController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Builds the GitHub App installation URL for connecting this project to a repository.
    /// </summary>
    /// <param name="organizationId">The organization's unique identifier.</param>
    /// <param name="projectId">The project's unique identifier.</param>
    [HttpGet("install-url")]
    [ProducesResponseType(typeof(GitHubInstallUrlResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GitHubInstallUrlResponse>> GetInstallUrl(
        Guid organizationId,
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var query = new GetGitHubInstallUrlQuery(organizationId, projectId);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(
            url => Ok(new GitHubInstallUrlResponse(url)),
            Problem);
    }

    /// <summary>
    /// Lists the repositories accessible to a GitHub App installation.
    /// </summary>
    /// <param name="organizationId">The organization's unique identifier.</param>
    /// <param name="projectId">The project's unique identifier.</param>
    /// <param name="installationId">The GitHub App installation's unique identifier.</param>
    /// <param name="state">The opaque CSRF state returned alongside the installation callback.</param>
    [HttpGet("installations/{installationId:long}/repositories")]
    [ProducesResponseType(typeof(IReadOnlyList<GitHubRepositoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<GitHubRepositoryResponse>>> GetInstallationRepositories(
        Guid organizationId,
        Guid projectId,
        long installationId,
        [FromQuery] string state,
        CancellationToken cancellationToken)
    {
        var query = new GetGitHubInstallationRepositoriesQuery(organizationId, projectId, installationId, state);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(
            repositories => Ok(MapToResponses(repositories)),
            Problem);
    }

    /// <summary>
    /// Connects this project to a GitHub repository.
    /// </summary>
    /// <param name="organizationId">The organization's unique identifier.</param>
    /// <param name="projectId">The project's unique identifier.</param>
    /// <param name="request">The repository to connect.</param>
    [HttpPost("connect")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Connect(
        Guid organizationId,
        Guid projectId,
        ConnectGitHubRepositoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ConnectGitHubRepositoryCommand(
            organizationId,
            projectId,
            request.InstallationId,
            request.RepositoryId,
            request.RepositoryOwner,
            request.RepositoryName);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Disconnects this project's GitHub repository, if any.
    /// </summary>
    /// <param name="organizationId">The organization's unique identifier.</param>
    /// <param name="projectId">The project's unique identifier.</param>
    [HttpDelete("connection")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Disconnect(
        Guid organizationId,
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var command = new DisconnectGitHubRepositoryCommand(organizationId, projectId);

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    /// <summary>
    /// Searches pull requests in this project's connected GitHub repository.
    /// </summary>
    /// <param name="organizationId">The organization's unique identifier.</param>
    /// <param name="projectId">The project's unique identifier.</param>
    /// <param name="q">The search term.</param>
    [HttpGet("pull-requests/search")]
    [ProducesResponseType(typeof(IReadOnlyList<GitHubPullRequestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<GitHubPullRequestResponse>>> SearchPullRequests(
        Guid organizationId,
        Guid projectId,
        [FromQuery] string q,
        CancellationToken cancellationToken)
    {
        var query = new SearchGitHubPullRequestsQuery(projectId, q);

        var result = await _sender.Send(query, cancellationToken);

        return result.Match(
            pullRequests => Ok(MapToResponses(pullRequests)),
            Problem);
    }

    private static List<GitHubRepositoryResponse> MapToResponses(IReadOnlyList<GitHubRepositorySummary> repositories)
    {
        return repositories
            .Select(repository => new GitHubRepositoryResponse(repository.Id, repository.Owner, repository.Name))
            .ToList();
    }

    private static List<GitHubPullRequestResponse> MapToResponses(IReadOnlyList<GitHubPullRequestSummary> pullRequests)
    {
        return pullRequests
            .Select(pr => new GitHubPullRequestResponse(
                pr.Id,
                pr.Number,
                pr.Title,
                pr.HtmlUrl,
                pr.IsOpen,
                pr.IsMerged,
                pr.AuthorLogin))
            .ToList();
    }
}
