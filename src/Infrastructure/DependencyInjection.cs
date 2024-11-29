using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Infrastructure.Authentication;
using Infrastructure.Authorization;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using AuthenticationOptions = Infrastructure.Authentication.AuthenticationOptions;
using AuthenticationService = Infrastructure.Authentication.AuthenticationService;
using IAuthenticationService = Application.Common.Interfaces.Services.IAuthenticationService;

namespace Infrastructure;

public static class DependencyInjection
{
    private const string ApiKeyHeaderName = "X-API-Key";

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddHttpContextAccessor()
            .AddServices(configuration)
            .AddAuthorization()
            .AddAuthentication(configuration)
            .AddPersistence(configuration)
            .AddHealthChecks(configuration)
            .AddCaching(configuration);

        return services;
    }

    private static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        var embeddingServiceOptions = configuration.GetSection(EmbeddingServiceOptions.SectionName);
        services.Configure<EmbeddingServiceOptions>(embeddingServiceOptions);

        services.AddHttpClient<IEmbeddingService, EmbeddingService>((serviceProvider, httpClient) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<EmbeddingServiceOptions>>().Value;
            httpClient.BaseAddress = new Uri(options.BaseUrl);
            httpClient.DefaultRequestHeaders.Add(ApiKeyHeaderName, options.ApiKey);
        });

        return services;
    }

    private static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString, options => options.UseVector()));

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

    private static IServiceCollection AddHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var keycloakServiceUri = new Uri(configuration["Keycloak:BaseUrl"]!);
        var embeddingServiceUri = new Uri($"{configuration["EmbeddingService:BaseUrl"]!}/health");

        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("Database")!)
            .AddRedis(configuration.GetConnectionString("Redis")!)
            .AddUrlGroup(keycloakServiceUri, HttpMethod.Get, "keycloak")
            .AddUrlGroup(embeddingServiceUri, HttpMethod.Get, "embedding");

        return services;
    }

    private static void AddCaching(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Redis");
        services.AddStackExchangeRedisCache(options => options.Configuration = connectionString);
        services.AddSingleton<ICacheService, CacheService>();
    }
}