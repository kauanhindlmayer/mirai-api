using Application.Abstractions;
using Application.Abstractions.Authentication;
using Application.Abstractions.Authorization;
using Application.Abstractions.Caching;
using Application.Abstractions.Email;
using Application.Abstractions.GitHub;
using Application.Abstractions.Jobs;
using Application.Abstractions.Links;
using Application.Abstractions.Storage;
using Application.Abstractions.Time;
using Asp.Versioning;
using Domain.Boards;
using Domain.Notifications;
using Domain.Organizations;
using Domain.Projects;
using Domain.Retrospectives;
using Domain.Shared;
using Domain.Sprints;
using Domain.Tags;
using Domain.Teams;
using Domain.Users;
using Domain.WikiPages;
using Domain.WorkItems;
using Infrastructure;
using Infrastructure.Authentication;
using Infrastructure.Authorization;
using Infrastructure.Caching;
using Infrastructure.Dashboards;
using Infrastructure.Email;
using Infrastructure.Integrations.GitHub;
using Infrastructure.Jobs;
using Infrastructure.Links;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Storage;
using Infrastructure.Time;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Octokit.Webhooks;
using Quartz;
using AuthenticationOptions = Infrastructure.Authentication.AuthenticationOptions;
using AuthenticationService = Infrastructure.Authentication.AuthenticationService;
using IAuthenticationService = Application.Abstractions.Authentication.IAuthenticationService;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddHttpContextAccessor()
            .AddServices(configuration)
            .AddAuthorization(configuration)
            .AddAuthentication(configuration)
            .AddGitHubApp(configuration)
            .AddPersistence(configuration)
            .AddApiVersioning()
            .AddCorsPolicy(configuration)
            .AddBackgroundJobs();

        return services;
    }

    public static WebApplicationBuilder AddOllamaServices(
        this WebApplicationBuilder builder)
    {
        builder
            .AddOllamaApiClient(ServiceKeys.Chat)
            .AddKeyedChatClient(ServiceKeys.Chat);
        builder
            .AddOllamaApiClient(ServiceKeys.Embeddings)
            .AddKeyedEmbeddingGenerator(ServiceKeys.Embeddings);

        return builder;
    }

    private static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IHtmlSanitizerService, HtmlSanitizerService>();
        services.AddTransient<LinkService>();
        services.AddTransient<IBackgroundJobScheduler, BackgroundJobScheduler>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IDashboardChartService, DashboardChartService>();
        services.AddSingleton<ICacheService, CacheService>();
        services.AddSingleton<IBlobService, BlobService>();
        services.Configure<BlobStorageOptions>(configuration.GetSection(BlobStorageOptions.SectionName));
        services.AddSingleton<IFrontendLinkService, FrontendLinkService>();
        services.Configure<FrontendOptions>(configuration.GetSection(FrontendOptions.SectionName));

        return services;
    }

    private static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<UpdateTimestampsInterceptor>();
        services.AddScoped<WorkItemChangeHistoryInterceptor>();
        services.AddScoped<DomainEventPublishingInterceptor>();

        var connectionString = configuration.GetConnectionString("mirai-db");
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
            options.UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.UseVector())
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(
                    sp.GetRequiredService<UpdateTimestampsInterceptor>(),
                    sp.GetRequiredService<WorkItemChangeHistoryInterceptor>(),
                    sp.GetRequiredService<DomainEventPublishingInterceptor>()));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IWorkItemRepository, WorkItemRepository>();
        services.AddScoped<IWikiPageRepository, WikiPageRepository>();
        services.AddScoped<IRetrospectiveRepository, RetrospectiveRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IBoardRepository, BoardRepository>();
        services.AddScoped<ISprintRepository, SprintRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    private static IServiceCollection AddAuthorization(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<AuthorizationService>();
        services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IPermissionService, PermissionService>();

        return services;
    }

    private static IServiceCollection AddAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var authenticationSection = configuration.GetSection(AuthenticationOptions.SectionName);
        services.Configure<AuthenticationOptions>(authenticationSection);

        var keycloakSection = configuration.GetSection(KeycloakOptions.SectionName);
        services.Configure<KeycloakOptions>(keycloakSection);
        var keycloakOptions = keycloakSection.Get<KeycloakOptions>()!;

        var gitHubSection = configuration.GetSection(GitHubIdentityProviderOptions.SectionName);
        services.Configure<GitHubIdentityProviderOptions>(gitHubSection);

        services
            .ConfigureOptions<JwtBearerOptionsSetup>()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.AddSingleton<IUserIdProvider, UserIdProvider>();

        services.AddTransient<AdminAuthorizationDelegatingHandler>();

        services.AddHttpClient<IAuthenticationService, AuthenticationService>((_, httpClient) =>
        {
            httpClient.BaseAddress = new Uri(keycloakOptions.AdminUrl);
        }).AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>();

        services.AddHttpClient<IIdentityProviderProvisioningService, KeycloakIdentityProviderProvisioningService>(
            (_, httpClient) =>
            {
                httpClient.BaseAddress = new Uri(keycloakOptions.AdminUrl);
            }).AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>();

        services.AddHttpClient<IJwtService, JwtService>((_, httpClient) =>
        {
            httpClient.BaseAddress = new Uri(keycloakOptions.TokenUrl);
        });

        return services;
    }

    private static IServiceCollection AddGitHubApp(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var gitHubAppSection = configuration.GetSection(GitHubAppOptions.SectionName);
        services.Configure<GitHubAppOptions>(gitHubAppSection);

        services.AddScoped<IGitHubAppClientFactory, GitHubAppClientFactory>();
        services.AddScoped<IGitHubInstallationService, GitHubInstallationService>();
        services.AddScoped<IGitHubPullRequestService, GitHubPullRequestService>();
        services.AddSingleton<IGitHubAppUrlProvider, GitHubAppUrlProvider>();
        services.AddScoped<WebhookEventProcessor, GitHubWebhookEventProcessor>();

        return services;
    }

    private static IServiceCollection AddApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1.0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionSelector = new DefaultApiVersionSelector(options);
            options.ApiVersionReader = ApiVersionReader.Combine(
                new MediaTypeApiVersionReader(),
                new MediaTypeApiVersionReaderBuilder()
                    .Template("application/vnd.mirai.hateoas.{version}+json")
                    .Build());
        })
        .AddMvc()
        .AddApiExplorer();

        return services;
    }

    private static IServiceCollection AddCorsPolicy(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var corsOptions = configuration
            .GetSection(CorsOptions.SectionName)
            .Get<CorsOptions>()!;

        services.AddCors(options => options.AddPolicy(CorsOptions.PolicyName, policy => policy
            .WithOrigins(corsOptions.AllowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()));

        return services;
    }

    private static IServiceCollection AddBackgroundJobs(this IServiceCollection services)
    {
        services.AddQuartz(config =>
        {
            config.AddJob<CleanupTagImportJobsJob>(job => job.WithIdentity("cleanup-tag-import-jobs"));

            config.AddTrigger(trigger =>
                trigger
                    .ForJob("cleanup-tag-import-jobs")
                    .WithIdentity("cleanup-tag-import-jobs-trigger")
                    .WithCronSchedule(
                        "0 0 3 * * ?",
                        cron => cron.InTimeZone(TimeZoneInfo.Utc)));

            config.AddJob<GitHubRepositorySyncJob>(job => job.WithIdentity("github-repository-sync"));

            config.AddTrigger(trigger =>
                trigger
                    .ForJob("github-repository-sync")
                    .WithIdentity("github-repository-sync-trigger")
                    .WithCronSchedule(
                        "0 */20 * * * ?",
                        cron => cron.InTimeZone(TimeZoneInfo.Utc)));
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        return services;
    }
}