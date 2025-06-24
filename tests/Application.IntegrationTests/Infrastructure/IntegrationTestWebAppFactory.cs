using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.Keycloak;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Application.IntegrationTests.Infrastructure;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("ankane/pgvector:latest")
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

    public async ValueTask InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();
        await _keycloakContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _redisContainer.StopAsync();
        await _keycloakContainer.StopAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:mirai-db", _dbContainer.GetConnectionString());
        builder.UseSetting("ConnectionStrings:mirai-redis", _redisContainer.GetConnectionString());

        var keycloakAddress = _keycloakContainer.GetBaseAddress();
        builder.UseSetting("Keycloak:AdminUrl", $"{keycloakAddress}admin/realms/mirai/");
        builder.UseSetting("Keycloak:TokenUrl", $"{keycloakAddress}realms/mirai/protocol/openid-connect/token");
        builder.UseSetting("Authentication:ValidIssuer", $"{keycloakAddress}realms/mirai/");
        builder.UseSetting("Authentication:MetadataAddress", $"{keycloakAddress}realms/mirai/.well-known/openid-configuration");
    }
}
