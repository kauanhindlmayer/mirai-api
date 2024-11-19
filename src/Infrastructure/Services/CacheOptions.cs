using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Services;

internal static class CacheOptions
{
    public static DistributedCacheEntryOptions DefaultExpiration => new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1),
    };

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
}