using Domain.Shared;

namespace Domain.Notifications.Events;

public sealed record NotificationCreatedDomainEvent(Notification Notification) : IDomainEvent;
