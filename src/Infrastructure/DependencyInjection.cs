using Application.Common.Interfaces;
using Domain.Common;
using GymManagement.Infrastructure.Authentication.PasswordHasher;
using Infrastructure.Boards.Persistence;
using Infrastructure.Common.Persistence;
using Infrastructure.Organizations.Persistence;
using Infrastructure.Projects.Persistence;
using Infrastructure.Retrospectives.Persistence;
using Infrastructure.Security;
using Infrastructure.Security.CurrentUserProvider;
using Infrastructure.Security.PolicyEnforcer;
using Infrastructure.Security.TokenGenerator;
using Infrastructure.Security.TokenValidation;
using Infrastructure.Services;
using Infrastructure.Tags.Persistence;
using Infrastructure.Teams.Persistence;
using Infrastructure.Users.Persistence;
using Infrastructure.WikiPages.Persistence;
using Infrastructure.WorkItems.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
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

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

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
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        services.AddSingleton<IPolicyEnforcer, PolicyEnforcer>();

        return services;
    }

    private static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Section));

        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        services
            .ConfigureOptions<JwtBearerTokenValidationConfiguration>()
            .AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        return services;
    }
}