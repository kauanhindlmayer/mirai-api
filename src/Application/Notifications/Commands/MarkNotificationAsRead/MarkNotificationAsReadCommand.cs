using ErrorOr;
using MediatR;

namespace Application.Notifications.Commands.MarkNotificationAsRead;

public sealed record MarkNotificationAsReadCommand(Guid NotificationId) : IRequest<ErrorOr<Success>>;
