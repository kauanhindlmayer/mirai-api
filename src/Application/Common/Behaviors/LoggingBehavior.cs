using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Application.Common.Behaviors;

internal sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting request {@RequestName}", typeof(TRequest).Name);

        var result = await next(cancellationToken);

        if (result.IsError)
        {
            using (LogContext.PushProperty("Error", result.Errors!.First(), true))
            {
                _logger.LogWarning("Request {@RequestName} failed with error", typeof(TRequest).Name);
            }
        }
        else
        {
            _logger.LogInformation("Request {@RequestName} succeeded", typeof(TRequest).Name);
        }

        return result;
    }
}