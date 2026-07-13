using Domain.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class NotificationRepository : Repository<Notification>, INotificationRepository
{
    public NotificationRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<List<Notification>> GetUnreadByRecipientIdAsync(
        Guid recipientUserId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Notifications
            .Where(n => n.RecipientUserId == recipientUserId && n.ReadAtUtc == null)
            .ToListAsync(cancellationToken);
    }
}
