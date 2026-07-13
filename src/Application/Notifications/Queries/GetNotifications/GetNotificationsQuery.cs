using Application.Abstractions;
using ErrorOr;
using MediatR;

namespace Application.Notifications.Queries.GetNotifications;

public sealed record GetNotificationsQuery(
    bool UnreadOnly,
    int Page,
    int PageSize) : IRequest<ErrorOr<PaginatedList<NotificationResponse>>>;
