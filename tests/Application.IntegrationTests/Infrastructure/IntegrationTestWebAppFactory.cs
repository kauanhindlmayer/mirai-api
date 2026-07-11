using Application.Abstractions.Authentication;
using Application.Abstractions.Storage;
using Domain.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.Keycloak;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Application.IntegrationTests.Infrastructure;

// TODO: Migrate from Testcontainers to Aspire.Hosting.Testing
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
        builder.UseSetting("ConnectionStrings:mirai-db", $"{_dbContainer.GetConnectionString()};Include Error Detail=true");
        builder.UseSetting("ConnectionStrings:redis", _redisContainer.GetConnectionString());

        var keycloakAddress = _keycloakContainer.GetBaseAddress();
        builder.UseSetting("Keycloak:AdminUrl", $"{keycloakAddress}admin/realms/mirai/");
        builder.UseSetting("Keycloak:TokenUrl", $"{keycloakAddress}realms/mirai/protocol/openid-connect/token");
        builder.UseSetting("Authentication:ValidIssuer", $"{keycloakAddress}realms/mirai/");
        builder.UseSetting("Authentication:MetadataAddress", $"{keycloakAddress}realms/mirai/.well-known/openid-configuration");

        builder.ConfigureTestServices(services =>
        {
            // Tests call handlers directly through ISender.Send with no HTTP pipeline, so the
            // real IUserContext (which reads the current request's claims) would always throw.
            services.Replace(ServiceDescriptor.Scoped<IUserContext, TestUserContext>());

            // No blob storage container is available in this test host.
            services.Replace(ServiceDescriptor.Singleton<IBlobService, FakeBlobService>());

            // No Ollama instance is available in this test host.
            services.RemoveAll<IEmbeddingGenerator<string, Embedding<float>>>();
            services.AddKeyedSingleton<IEmbeddingGenerator<string, Embedding<float>>, FakeEmbeddingGenerator>(
                ServiceKeys.Embeddings);
        });
    }
}
