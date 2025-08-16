using System.Text.Json;
using Application.Abstractions.Caching;
using StackExchange.Redis;

namespace Infrastructure.Caching;

internal sealed class CacheService : ICacheService
{
    private readonly IDatabase _db;
    private readonly JsonSerializerOptions _jsonOptions;

    public CacheService(IConnectionMultiplexer connection)
    {
        _db = connection.GetDatabase();
        _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        _jsonOptions.Converters.Add(new ErrorOrJsonConverterFactory());
    }

    public async Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default)
    {
        var value = await _db.StringGetAsync(key);
        if (value.IsNullOrEmpty)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(value.ToString(), _jsonOptions);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        string json = JsonSerializer.Serialize(value, _jsonOptions);
        await _db.StringSetAsync(key, json, expiration);
    }

    public Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        return _db.KeyDeleteAsync(key);
    }
}
