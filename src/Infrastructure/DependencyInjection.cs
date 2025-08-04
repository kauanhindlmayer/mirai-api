using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Services;
using Asp.Versioning;
using Azure.Storage.Blobs;
using Infrastructure.Authentication;
using Infrastructure.Authorization;
using Infrastructure.Jobs;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.Nlp;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using AuthenticationOptions = Infrastructure.Settings.AuthenticationOptions;
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
            .AddApiVersioning()
            .AddCorsPolicy(configuration)
            .AddCaching(configuration)
            .AddBackgroundJobs();

        return services;
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

        var nlpServiceOptions = configuration.GetSection(NlpServiceOptions.SectionName);
        services.Configure<NlpServiceOptions>(nlpServiceOptions);

        services.AddHttpClient<INlpService, NlpService>((serviceProvider, httpClient) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<NlpServiceOptions>>().Value;
            httpClient.BaseAddress = new Uri(options.BaseUrl);
            httpClient.DefaultRequestHeaders.Add(ApiKeyHeaderName, options.ApiKey);
        });

        services.AddSingleton<IBlobService, BlobService>();
        services.AddSingleton(_ => new BlobServiceClient(configuration["Azure:BlobStorage:ConnectionString"]!));
        services.Configure<BlobStorageOptions>(configuration.GetSection(BlobStorageOptions.SectionName));

        return services;
    }

    private static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("mirai-db");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.UseVector())
                .UseSnakeCaseNamingConvention());

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
        services.AddScoped<ISprintsRepository, SprintsRepository>();

        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());

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
        var authenticationSection = configuration.GetSection(AuthenticationOptions.SectionName);
        services.Configure<AuthenticationOptions>(authenticationSection);

        var keycloakSection = configuration.GetSection(KeycloakOptions.SectionName);
        services.Configure<KeycloakOptions>(keycloakSection);
        var keycloakOptions = keycloakSection.Get<KeycloakOptions>()!;

        services
            .ConfigureOptions<JwtBearerOptionsSetup>()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.AddTransient<AdminAuthorizationDelegatingHandler>();

        services.AddHttpClient<IAuthenticationService, AuthenticationService>((_, httpClient) =>
        {
            httpClient.BaseAddress = new Uri(keycloakOptions.AdminUrl);
        }).AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>();

        services.AddHttpClient<IJwtService, JwtService>((_, httpClient) =>
        {
            httpClient.BaseAddress = new Uri(keycloakOptions.TokenUrl);
        });

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
            .AllowAnyHeader()));

        return services;
    }

    private static IServiceCollection AddCaching(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("mirai-redis");
        services.AddStackExchangeRedisCache(options => options.Configuration = connectionString);
        services.AddSingleton<ICacheService, CacheService>();
        return services;
    }

    private static IServiceCollection AddBackgroundJobs(this IServiceCollection services)
    {
        services.AddQuartz(config =>
        {
            // Runs every day at 3 AM UTC
            config.AddJob<CleanupTagImportJobsJob>(job => job.WithIdentity("cleanup-tag-import-jobs"));

            config.AddTrigger(trigger =>
                trigger
                    .ForJob("cleanup-tag-import-jobs")
                    .WithIdentity("cleanup-tag-import-jobs-trigger")
                    .WithCronSchedule(
                        "0 0 3 * * ?",
                        cron => cron.InTimeZone(TimeZoneInfo.Utc)));
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        return services;
    }
}