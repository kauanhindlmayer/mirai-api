using System.Data.Common;
using Application.Abstractions.Email;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Infrastructure.Email;

public sealed class EmailService : IEmailService
{
    private const string FromAddress = "noreply@miraihq.com";
    private const string FromName = "Mirai";

    private readonly ILogger<EmailService> _logger;
    private readonly Uri? _smtpEndpoint;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _logger = logger;
        _smtpEndpoint = ResolveSmtpEndpoint(configuration);
    }

    public async Task SendEmailAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Sending email to {To} with subject {Subject}",
            to,
            subject);

        if (_smtpEndpoint is null)
        {
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(FromName, FromAddress));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(
            _smtpEndpoint.Host,
            _smtpEndpoint.Port,
            SecureSocketOptions.None,
            cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }

    private static Uri? ResolveSmtpEndpoint(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("mailpit");

        if (connectionString is null)
        {
            return null;
        }

        var connectionStringBuilder = new DbConnectionStringBuilder
        {
            ConnectionString = connectionString,
        };

        return new Uri(connectionStringBuilder["Endpoint"].ToString()!, UriKind.Absolute);
    }
}
