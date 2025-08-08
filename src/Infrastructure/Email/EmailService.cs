using Application.Abstractions.Email;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Email;

public sealed class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Sending email to {To} with subject {Subject}",
            to,
            subject);

        return Task.CompletedTask;
    }
}
