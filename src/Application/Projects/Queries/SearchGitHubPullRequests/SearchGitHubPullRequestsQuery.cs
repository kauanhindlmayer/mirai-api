using Application.Abstractions.GitHub;
using ErrorOr;
using MediatR;

namespace Application.Projects.Queries.SearchGitHubPullRequests;

public sealed record SearchGitHubPullRequestsQuery(
    Guid ProjectId,
    string SearchTerm) : IRequest<ErrorOr<IReadOnlyList<GitHubPullRequestSummary>>>;
