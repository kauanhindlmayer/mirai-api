using Application.Common.Behaviors;
using Application.Common.Mappings;
using Application.Common.Sorting;
using Application.Tags.Queries.ListTags;
using Application.WorkItems.Queries.Common;
using Domain.Tags;
using Domain.WorkItems;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(QueryCachingBehavior<,>));
            config.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));
        });
        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));

        services.AddTransient<SortMappingProvider>();
        services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<WorkItemBriefResponse, WorkItem>>(_ =>
            WorkItemMappings.SortMapping);
        services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<TagResponse, Tag>>(_ =>
            TagMappings.SortMapping);

        return services;
    }
}