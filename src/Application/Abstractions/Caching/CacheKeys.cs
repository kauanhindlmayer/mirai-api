namespace Application.Abstractions.Caching;

public static class CacheKeys
{
    public static string GetOrganizationsKey()
        => "organizations";
    public static string GetOrganizationKey(Guid organizationId)
        => $"organization:{organizationId}";
    public static string GetDashboardKey(Guid teamId, DateTime startDate, DateTime endDate)
        => $"dashboard:{teamId}:{startDate:yyyy-MM-dd}:{endDate:yyyy-MM-dd}";
}
