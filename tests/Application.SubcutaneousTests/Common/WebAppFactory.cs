using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WebApi;

namespace Application.SubcutaneousTests.Common;

public class WebAppFactory : WebApplicationFactory<IWebApiAssemblyMarker>, IAsyncLifetime
{
    public SqliteTestDatabase TestDatabase { get; set; } = null!;

    public IMediator CreateMediator()
    {
        var serviceScope = Services.CreateScope();

        TestDatabase.ResetDatabase();

        return serviceScope.ServiceProvider.GetRequiredService<IMediator>();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public new Task DisposeAsync()
    {
        TestDatabase.Dispose();

        return Task.CompletedTask;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        TestDatabase = SqliteTestDatabase.CreateAndInitialize();

        builder.ConfigureTestServices(services =>
        {
            // TODO: Replace the current usr provider implementation with the user context implementation
            // services
            //     .RemoveAll<ICurrentUserProvider>()
            //     .AddScoped<ICurrentUserProvider>(_ => TestCurrentUserProvider);
            services
                .RemoveAll<DbContextOptions<ApplicationDbContext>>()
                .AddDbContext<ApplicationDbContext>((sp, options) => options.UseSqlite(TestDatabase.Connection));
        });

        builder.ConfigureAppConfiguration((context, conf) => conf.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "EmailSettings:EnableEmailNotifications", "false" },
        }));
    }
}