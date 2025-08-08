namespace Application.Abstractions.Email;

public interface IEmailService
{
    Task SendEmailAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default);
}