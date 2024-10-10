using Application.Common.Interfaces;
using Infrastructure.Authentication;
using Infrastructure.Authorization;
using Infrastructure.Boards.Persistence;
using Infrastructure.Common.Persistence;
using Infrastructure.Organizations.Persistence;
using Infrastructure.Projects.Persistence;
using Infrastructure.Retrospectives.Persistence;
using Infrastructure.Services;
using Infrastructure.Tags.Persistence;
using Infrastructure.Teams.Persistence;
using Infrastructure.Users.Persistence;
using Infrastructure.WikiPages.Persistence;
using Infrastructure.WorkItems.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using AuthenticationOptions = Infrastructure.Authentication.AuthenticationOptions;
using AuthenticationService = Infrastructure.Authentication.AuthenticationService;
using IAuthenticationService = Application.Common.Interfaces.IAuthenticationService;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddHttpContextAccessor()
            .AddServices()
            .AddAuthorization()
            .AddAuthentication(configuration)
            .AddPersistence(configuration);

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        return services;
    }

    private static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration[DbConstants.DefaultConnectionStringPath];
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IOrganizationsRepository, OrganizationsRepository>();
        services.AddScoped<IProjectsRepository, ProjectsRepository>();
        services.AddScoped<IWorkItemsRepository, WorkItemsRepository>();
        services.AddScoped<IWikiPagesRepository, WikiPagesRepository>();
        services.AddScoped<IRetrospectivesRepository, RetrospectivesRepository>();
        services.AddScoped<ITeamsRepository, TeamsRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<ITagsRepository, TagsRepository>();
        services.AddScoped<IBoardsRepository, BoardsRepository>();

        return services;
    }

    private static IServiceCollection AddAuthorization(this IServiceCollection services)
    {
        services.AddScoped<AuthorizationService>();
        services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();
        services.AddScoped<IUserContext, UserContext>();

        return services;
    }

    private static IServiceCollection AddAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var authenticationOptions = configuration.GetSection(AuthenticationOptions.SectionName);
        services.Configure<AuthenticationOptions>(authenticationOptions);

        var keycloakOptions = configuration.GetSection(KeycloakOptions.SectionName);
        services.Configure<KeycloakOptions>(keycloakOptions);

        services
            .ConfigureOptions<JwtBearerOptionsSetup>()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.AddTransient<AdminAuthorizationDelegatingHandler>();

        services.AddHttpClient<IAuthenticationService, AuthenticationService>((serviceProvider, httpClient) =>
        {
            var keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;
            httpClient.BaseAddress = new Uri(keycloakOptions.AdminUrl);
        }).AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>();

        services.AddHttpClient<IJwtService, JwtService>((serviceProvider, httpClient) =>
        {
            var keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;
            httpClient.BaseAddress = new Uri(keycloakOptions.TokenUrl);
        });

        return services;
    }
}