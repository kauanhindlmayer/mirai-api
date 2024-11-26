using Application.Common.Interfaces.Services;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors;

internal sealed class QueryCachingBehavior<TRequest, TResponse>(
    ICacheService cacheService,
    ILogger<QueryCachingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICachedQuery
    where TResponse : IErrorOr
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var cachedResult = await cacheService.GetAsync<TResponse>(
            request.CacheKey,
            cancellationToken);

        var queryName = typeof(TRequest).Name;

        if (IsValidCachedResult(cachedResult))
        {
            logger.LogInformation("Cache hit for query {@QueryName}", queryName);
            return cachedResult!;
        }

        logger.LogInformation("Cache miss for query {@QueryName}", queryName);

        var result = await next();

        if (result.IsError)
        {
            return result;
        }

        await cacheService.SetAsync(
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