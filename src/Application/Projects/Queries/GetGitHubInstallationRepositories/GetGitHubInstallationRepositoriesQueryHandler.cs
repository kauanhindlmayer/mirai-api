using Application.Abstractions.Caching;
using Application.Abstractions.GitHub;
using Application.Projects.GitHub;
using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Projects.Queries.GetGitHubInstallationRepositories;

internal sealed class GetGitHubInstallationRepositoriesQueryHandler
    : IRequestHandler<GetGitHubInstallationRepositoriesQuery, ErrorOr<IReadOnlyList<GitHubRepositorySummary>>>
{
    private readonly ICacheService _cacheService;
    private readonly IGitHubInstallationService _installationService;

    public GetGitHubInstallationRepositoriesQueryHandler(
        ICacheService cacheService,
        IGitHubInstallationService installationService)
    {
        _cacheService = cacheService;
        _installationService = installationService;
    }

    public async Task<ErrorOr<IReadOnlyList<GitHubRepositorySummary>>> Handle(
        GetGitHubInstallationRepositoriesQuery query,
        CancellationToken cancellationToken)
    {
        var stateKey = CacheKeys.GetGitHubInstallationStateKey(query.State);
        var stateData = await _cacheService.GetAsync<GitHubInstallationState>(stateKey, cancellationToken);

        if (stateData is null ||
            stateData.OrganizationId != query.OrganizationId ||
            stateData.ProjectId != query.ProjectId)
        {
            return ProjectErrors.InvalidGitHubInstallationState;
        }

        var repositories = await _installationService.ListRepositoriesAsync(query.InstallationId, cancellationToken);
        return repositories.ToList();
    }
}
