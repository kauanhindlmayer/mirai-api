namespace Application.Common.Interfaces.Services;

public interface IEmailService
{
    Task SendEmailAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default);
}