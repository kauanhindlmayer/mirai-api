using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Mirai.Application.Common.Behaviors;

public sealed class RequestLoggingPipelineBehavior<TRequest, TResponse>(
    ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> _logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Starting request {@RequestName}",
            typeof(TRequest).Name);

        var result = await next();

        if (result.IsError)
        {
            using (LogContext.PushProperty("Error", result.Errors!.First(), true))
            {
                _logger.LogWarning(
                    "Request {@RequestName} failed with error",
                    typeof(TRequest).Name);
            }
        }
        else
        {
            _logger.LogInformation(
                "Request {@RequestName} succeeded",
                typeof(TRequest).Name);
        }

        return result;
    }
}