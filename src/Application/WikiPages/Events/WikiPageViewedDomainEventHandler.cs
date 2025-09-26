using Application.Abstractions;
using Domain.WikiPages;
using Domain.WikiPages.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.WikiPages.Events;

internal sealed class WikiPageViewedDomainEventHandler
    : INotificationHandler<WikiPageViewedDomainEvent>
{
    private readonly IApplicationDbContext _context;

    public WikiPageViewedDomainEventHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        WikiPageViewedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        var hasRecentView = await _context.WikiPageViews
            .AnyAsync(
                v =>
                v.WikiPageId == notification.WikiPageId &&
                v.ViewerId == notification.ViewerId &&
                v.ViewedAt > DateTime.UtcNow.AddMinutes(-30),
                cancellationToken);

        if (!hasRecentView)
        {
            var view = new WikiPageView(
                notification.WikiPageId,
                notification.ViewerId);
            _context.WikiPageViews.Add(view);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}