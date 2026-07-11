using Application.Abstractions.Authentication;
using Application.Abstractions.Storage;
using Domain.Authorization;
using Domain.Users;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.IntegrationTests.Infrastructure;

[Collection(nameof(IntegrationTestCollection))]
public abstract class BaseIntegrationTest
{
    protected readonly ISender _sender;
    protected readonly ApplicationDbContext _dbContext;
    private readonly IServiceScope _scope;

    public BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        _sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    /// <summary>
    /// Sets the user that <see cref="IUserContext"/> reports as the caller for
    /// handlers invoked through <see cref="_sender"/> in this test.
    /// </summary>
    protected void SetCurrentUser(Guid userId)
    {
        var userContext = (TestUserContext)_scope.ServiceProvider.GetRequiredService<IUserContext>();
        userContext.UserId = userId;
    }

    /// <summary>
    /// Fetches a system role tracked by this test's <see cref="_dbContext"/> - passing
    /// a <see cref="SystemRoles"/> static instance directly to a domain method instead
    /// would make EF treat it as new and try to re-insert a row that already exists
    /// from the migration's seeded data.
    /// </summary>
    protected Task<Role> GetRoleAsync(Guid roleId)
    {
        return _dbContext.Roles.SingleAsync(r => r.Id == roleId);
    }

    /// <summary>
    /// The in-memory fake replacing IBlobService in this test host - use to assert a file was
    /// (or wasn't) deleted, since no real blob storage container is available here.
    /// </summary>
    protected FakeBlobService FakeBlobService =>
        (FakeBlobService)_scope.ServiceProvider.GetRequiredService<IBlobService>();

    /// <summary>
    /// Seeds and persists a user, then sets it as the current caller via <see cref="SetCurrentUser"/>.
    /// Required before saving a new <see cref="Domain.Projects.Project"/> directly through
    /// <see cref="_dbContext"/>: its creation raises a domain event that creates a default wiki page
    /// authored by the current user, which fails its foreign key check if no caller has been set.
    /// </summary>
    protected async Task<User> SeedCurrentUserAsync()
    {
        var user = new User("Current", "User", $"current-{Guid.NewGuid()}@mirai.com");
        user.SetIdentityId(Guid.NewGuid().ToString());
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        SetCurrentUser(user.Id);

        return user;
    }
}