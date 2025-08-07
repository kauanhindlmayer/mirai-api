using Application.Abstractions.Email;
using Domain.Projects.Events;
using MediatR;

namespace Application.Projects.Events;

internal sealed class UserAddedToProjectDomainEventHandler : INotificationHandler<UserAddedToProjectDomainEvent>
{
    private readonly IEmailService _emailService;

    public UserAddedToProjectDomainEventHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Handle(
        UserAddedToProjectDomainEvent notification,
        CancellationToken cancellationToken)
    {
        await _emailService.SendEmailAsync(
            notification.User.Email,
            "Added to Project",
            $"""
            Hello {notification.User.FullName},
            
            You have been added to the project '{notification.Project.Name}'.
            """,
            cancellationToken);
    }
}