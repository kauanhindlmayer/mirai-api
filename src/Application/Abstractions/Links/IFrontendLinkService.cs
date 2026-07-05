namespace Application.Abstractions.Links;

public interface IFrontendLinkService
{
    string BuildResetPasswordLink(string email, string token);
}
