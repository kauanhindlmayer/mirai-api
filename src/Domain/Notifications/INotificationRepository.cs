using Domain.Shared;

namespace Domain.Notifications;

public interface INotificationRepository : IRepository<Notification>
{
    Task<List<Notification>> GetUnreadByRecipientIdAsync(
        Guid recipientUserId,
        CancellationToken cancellationToken = default);
}
