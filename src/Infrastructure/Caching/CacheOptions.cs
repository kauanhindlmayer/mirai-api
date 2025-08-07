using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Caching;

internal static class CacheOptions
{
    public static DistributedCacheEntryOptions Create(TimeSpan? expiration = null)
    {
        if (expiration is null)
        {
            return DefaultExpiration;
        }

        return new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration,
        };
    }

    private static DistributedCacheEntryOptions DefaultExpiration => new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1),
    };
}