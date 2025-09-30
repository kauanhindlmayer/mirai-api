using Application.Abstractions.Behaviors;
using Application.Abstractions.Mappings;
using Application.Abstractions.Sorting;
using Application.Organizations.Queries.GetOrganizationUsers;
using Application.Projects.Queries.GetProjectUsers;
using Application.Tags.Queries.ListTags;
using Application.WorkItems.Queries.ListWorkItems;
using Domain.Tags;
using Domain.Users;
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
        services.AddValidatorsFromAssemblyContaining(
            typeof(DependencyInjection),
            includeInternalTypes: true);

        services.AddTransient<SortMappingProvider>();
        services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<WorkItemBriefResponse, WorkItem>>(_ =>
            WorkItemMappings.SortMapping);
        services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<TagResponse, Tag>>(_ =>
            TagMappings.SortMapping);
        services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<OrganizationUserResponse, User>>(_ =>
            OrganizationUserMappings.SortMapping);
        services.AddSingleton<ISortMappingDefinition, SortMappingDefinition<ProjectUserResponse, User>>(_ =>
            ProjectUserMappings.SortMapping);

        return services;
    }
}