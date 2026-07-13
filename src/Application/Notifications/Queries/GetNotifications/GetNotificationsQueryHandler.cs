using Application.Abstractions;
using Application.Abstractions.Authentication;
using Application.Abstractions.Mappings;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Notifications.Queries.GetNotifications;

internal sealed class GetNotificationsQueryHandler
    : IRequestHandler<GetNotificationsQuery, ErrorOr<PaginatedList<NotificationResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;

    public GetNotificationsQueryHandler(
        IApplicationDbContext context,
        IUserContext userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    public async Task<ErrorOr<PaginatedList<NotificationResponse>>> Handle(
        GetNotificationsQuery query,
        CancellationToken cancellationToken)
    {
        var notifications = _context.Notifications
            .AsNoTracking()
            .Where(n => n.RecipientUserId == _userContext.UserId);

        if (query.UnreadOnly)
        {
            notifications = notifications.Where(n => n.ReadAtUtc == null);
        }

        return await notifications
            .OrderByDescending(n => n.CreatedAtUtc)
            .Select(n => new NotificationResponse
            {
                Id = n.Id,
                Type = n.Type,
                EntityId = n.EntityId,
                Message = n.Message,
                ReadAtUtc = n.ReadAtUtc,
                CreatedAtUtc = n.CreatedAtUtc,
            })
            .PaginatedListAsync(query.Page, query.PageSize, cancellationToken);
    }
}
