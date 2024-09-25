using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Mirai.Application.Common.Behaviors;

namespace Mirai.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            options.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
            options.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
            options.AddOpenBehavior(typeof(ValidationBehavior<,>));
            options.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));
        });

        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));
        return services;
    }
}