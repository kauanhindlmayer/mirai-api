using System.Text;
using Application.Abstractions.Authentication;
using Application.Abstractions.Caching;
using Application.Abstractions.GitHub;
using Application.Projects.GitHub;
using Domain.Projects;
using ErrorOr;
using MediatR;

namespace Application.Projects.Queries.GetGitHubInstallUrl;

internal sealed class GetGitHubInstallUrlQueryHandler
    : IRequestHandler<GetGitHubInstallUrlQuery, ErrorOr<string>>
{
    private static readonly TimeSpan StateExpiration = TimeSpan.FromMinutes(10);

    private readonly IProjectRepository _projectRepository;
    private readonly IGitHubAppUrlProvider _urlProvider;
    private readonly ICacheService _cacheService;
    private readonly IUserContext _userContext;

    public GetGitHubInstallUrlQueryHandler(
        IProjectRepository projectRepository,
        IGitHubAppUrlProvider urlProvider,
        ICacheService cacheService,
        IUserContext userContext)
    {
        _projectRepository = projectRepository;
        _urlProvider = urlProvider;
        _cacheService = cacheService;
        _userContext = userContext;
    }

    public async Task<ErrorOr<string>> Handle(
        GetGitHubInstallUrlQuery query,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(query.ProjectId, cancellationToken);
        if (project is null || project.OrganizationId != query.OrganizationId)
        {
            return ProjectErrors.NotFound;
        }

        // GitHub's installation Setup URL is static and app-level (no room for
        // path parameters), so the org/project ids the callback needs travel
        // inside this opaque state value itself; the cache entry below is the
        // authoritative check (expiry + tamper/replay protection), this
        // encoding just lets the frontend recover which project it's for.
        var state = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{query.OrganizationId}|{query.ProjectId}|{Guid.NewGuid():N}"));
        var stateData = new GitHubInstallationState(query.OrganizationId, query.ProjectId, _userContext.UserId);

        await _cacheService.SetAsync(
            CacheKeys.GetGitHubInstallationStateKey(state),
            stateData,
            StateExpiration,
            cancellationToken);

        return _urlProvider.BuildInstallUrl(state);
    }
}
