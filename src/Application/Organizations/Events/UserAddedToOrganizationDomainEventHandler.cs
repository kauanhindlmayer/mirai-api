using Application.Common.Interfaces.Services;
using Domain.Organizations.Events;
using MediatR;

namespace Application.Organizations.Events;

internal sealed class UserAddedToOrganizationDomainEventHandler
    : INotificationHandler<UserAddedToOrganizationDomainEvent>
{
    private readonly IEmailService _emailService;

    public UserAddedToOrganizationDomainEventHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Handle(
        UserAddedToOrganizationDomainEvent notification,
        CancellationToken cancellationToken)
    {
        await _emailService.SendEmailAsync(
            notification.User.Email,
            "Welcome to the Organization",
            $"""
            Hello {notification.User.FullName},
            
            You have been added to the organization '{notification.Organization.Name}'.
            """,
            cancellationToken);
    }
}