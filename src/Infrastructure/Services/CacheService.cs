using System.Buffers;
using System.Text.Json;
using Application.Common.Interfaces.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Services;

internal sealed class CacheService(IDistributedCache cache) : ICacheService
{
    public async Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default)
    {
        var bytes = await cache.GetAsync(key, cancellationToken);
        return bytes is null ? default : Deserialize<T>(bytes);
    }

    public Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        byte[] bytes = Serialize(value);

        return cache.SetAsync(
            key,
            bytes,
            CacheOptions.Create(expiration),
            cancellationToken);
    }

    public Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        return cache.RemoveAsync(key, cancellationToken);
    }

    private static T Deserialize<T>(byte[] bytes)
    {
        var result = JsonSerializer.Deserialize<T>(bytes);
        return result!;
    }

    private static byte[] Serialize<T>(T value)
    {
        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);
        JsonSerializer.Serialize(writer, value);
        return buffer.WrittenSpan.ToArray();
    }
}
