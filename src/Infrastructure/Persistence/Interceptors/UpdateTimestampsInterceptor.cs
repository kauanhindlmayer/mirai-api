using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence.Interceptors;

internal sealed class UpdateTimestampsInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            UpdateTimestamps(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void UpdateTimestamps(DbContext context)
    {
        var entities = context.ChangeTracker.Entries<Entity>()
            .Where(entry => entry.State == EntityState.Modified)
            .Select(entry => entry.Entity);

        foreach (var entity in entities)
        {
            typeof(Entity)
                .GetProperty(nameof(Entity.UpdatedAtUtc))!
                .SetValue(entity, DateTime.UtcNow);
        }
    }
}
