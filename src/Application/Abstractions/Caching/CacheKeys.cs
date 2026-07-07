namespace Application.Abstractions.Caching;

public static class CacheKeys
{
    public static string GetOrganizationsKey(Guid userId)
        => $"organizations:{userId}";
    public static string GetOrganizationKey(Guid organizationId)
        => $"organization:{organizationId}";
    public static string GetDashboardKey(Guid teamId, DateTime startDate, DateTime endDate)
        => $"dashboard:{teamId}:{startDate:yyyy-MM-dd}:{endDate:yyyy-MM-dd}";
    public static string GetGitHubInstallationTokenKey(long installationId)
        => $"github-installation-token:{installationId}";
    public static string GetGitHubInstallationStateKey(string state)
        => $"github-installation-state:{state}";
}
