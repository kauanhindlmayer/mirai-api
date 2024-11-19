using Application.Common.Caching;
using Application.Common.Interfaces;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors;

internal sealed class QueryCachingBehavior<TRequest, TResponse>(
    ICacheService _cacheService,
    ILogger<QueryCachingBehavior<TRequest, TResponse>> _logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICachedQuery
    where TResponse : struct, IErrorOr
{
    private readonly ICacheService _cacheService = _cacheService;
    private readonly ILogger<QueryCachingBehavior<TRequest, TResponse>> _logger = _logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // TODO: Refactor to avoid race conditions
        var cachedResult = await _cacheService.GetAsync<TResponse>(
            request.CacheKey,
            cancellationToken);

        var queryName = typeof(TRequest).Name;

        if (cachedResult is not null)
        {
            _logger.LogInformation("Cache hit for query {@QueryName}", queryName);
            return (TResponse)cachedResult;
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
}