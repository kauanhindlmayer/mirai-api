namespace Application.Common.Caching;

public static class CacheKeys
{
    public static string GetOrganizationsKey()
        => "organizations";
    public static string GetOrganizationKey(Guid organizationId)
        => $"organization:{organizationId}";
}
