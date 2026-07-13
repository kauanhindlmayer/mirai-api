using Application.Abstractions;
using Domain.Notifications;
using Domain.Organizations.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Notifications.Events;

internal sealed class OrganizationDeletedNotificationHandler
    : INotificationHandler<OrganizationDeletedDomainEvent>
{
    private readonly IApplicationDbContext _context;

    public OrganizationDeletedNotificationHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        OrganizationDeletedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        var notifications = await _context.Notifications
            .Where(n =>
                n.Type == NotificationType.AddedToOrganization &&
                n.EntityId == notification.Organization.Id)
            .ToListAsync(cancellationToken);

        if (notifications.Count == 0)
        {
            return;
        }

        _context.Notifications.RemoveRange(notifications);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
