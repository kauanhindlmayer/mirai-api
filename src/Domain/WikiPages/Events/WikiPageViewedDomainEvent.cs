using MediatR;

namespace Domain.WikiPages.Events;

public sealed record WikiPageViewedDomainEvent(
    Guid WikiPageId,
    Guid ViewerId) : INotification;