using Application.Common.Interfaces.Services;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors;

internal sealed class QueryCachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICachedQuery
    where TResponse : IErrorOr
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<QueryCachingBehavior<TRequest, TResponse>> _logger;

    public QueryCachingBehavior(
        ICacheService cacheService,
        ILogger<QueryCachingBehavior<TRequest, TResponse>> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var cachedResult = await _cacheService.GetAsync<TResponse>(
            request.CacheKey,
            cancellationToken);

        var queryName = typeof(TRequest).Name;

        if (IsValidCachedResult(cachedResult))
        {
            _logger.LogInformation("Cache hit for query {@QueryName}", queryName);
            return cachedResult!;
        }

        _logger.LogInformation("Cache miss for query {@QueryName}", queryName);

        var result = await next();
        if (result.IsError)
        {
            return result;
        }

        await _cacheService.SetAsync(
            request.CacheKey,
            result,
            request.Expiration,
            cancellationToken);

        return result;
    }

    private static bool IsValidCachedResult<T>(T cachedResult)
    {
        if (cachedResult is null)
        {
            return false;
        }

        var value = cachedResult.GetType().GetProperty("Value")?.GetValue(cachedResult);

        return value is not null;
    }
}