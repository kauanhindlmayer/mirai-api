using Application.Abstractions.GitHub;
using ErrorOr;
using MediatR;

namespace Application.Projects.Queries.GetGitHubInstallationRepositories;

public sealed record GetGitHubInstallationRepositoriesQuery(
    Guid OrganizationId,
    Guid ProjectId,
    long InstallationId,
    string State) : IRequest<ErrorOr<IReadOnlyList<GitHubRepositorySummary>>>;
