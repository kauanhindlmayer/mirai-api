using ErrorOr;
using MediatR;

namespace Application.Notifications.Commands.MarkAllNotificationsAsRead;

public sealed record MarkAllNotificationsAsReadCommand : IRequest<ErrorOr<Success>>;
