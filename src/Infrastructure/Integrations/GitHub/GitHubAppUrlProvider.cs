using System.Web;
using Application.Abstractions.GitHub;
using Microsoft.Extensions.Options;

namespace Infrastructure.Integrations.GitHub;

internal sealed class GitHubAppUrlProvider : IGitHubAppUrlProvider
{
    private readonly GitHubAppOptions _options;

    public GitHubAppUrlProvider(IOptions<GitHubAppOptions> options)
    {
        _options = options.Value;
    }

    public string BuildInstallUrl(string state)
    {
        var encodedState = HttpUtility.UrlEncode(state);
        return $"https://github.com/apps/{_options.InstallUrlSlug}/installations/new?state={encodedState}";
    }
}
