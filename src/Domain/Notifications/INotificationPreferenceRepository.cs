using Domain.Shared;

namespace Domain.Notifications;

public interface INotificationPreferenceRepository : IRepository<NotificationPreference>
{
    Task<NotificationPreference?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
