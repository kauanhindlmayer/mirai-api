using System.Net.Http.Json;
using Infrastructure.Authentication;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.Keycloak;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using WebApi.FunctionalTests.Users;

namespace WebApi.FunctionalTests.Common;

public class FunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("mirai-db")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:latest")
        .Build();

    private readonly KeycloakContainer _keycloakContainer = new KeycloakBuilder()
        .WithResourceMapping(
            new FileInfo(".files/mirai-realm-export.json"),
            new FileInfo("/opt/keycloak/data/import/realm.json"))
        .WithCommand("--import-realm")
        .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();
        await _keycloakContainer.StartAsync();

        await InitializeTestUserAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _redisContainer.StopAsync();
        await _keycloakContainer.StopAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));

            string connectionString = $"{_dbContainer.GetConnectionString()};Pooling=False";

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.Configure<RedisCacheOptions>(options =>
                options.Configuration = _redisContainer.GetConnectionString());

            string? keycloakAddress = _keycloakContainer.GetBaseAddress();

            services.Configure<KeycloakOptions>(options =>
            {
                options.AdminUrl = $"{keycloakAddress}admin/realms/mirai/";
                options.TokenUrl = $"{keycloakAddress}realms/mirai/protocol/openid-connect/token";
            });

            services.Configure<AuthenticationOptions>(options =>
            {
                options.ValidIssuer = $"{keycloakAddress}realms/mirai/";
                options.MetadataAddress = $"{keycloakAddress}realms/mirai/.well-known/openid-configuration";
            });
        });
    }

    private async Task InitializeTestUserAsync()
    {
        try
        {
            var logs = await _keycloakContainer.GetLogsAsync();
            using HttpClient httpClient = CreateClient();
            await httpClient.PostAsJsonAsync("api/users/register", UserRequestFactory.CreateRegisterUserRequest());
        }
        catch
        {
            // ignored
        }
    }
}
