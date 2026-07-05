using Application.Abstractions.Links;
using Microsoft.Extensions.Options;

namespace Infrastructure.Links;

internal sealed class FrontendLinkService : IFrontendLinkService
{
    private readonly FrontendOptions _frontendOptions;

    public FrontendLinkService(IOptions<FrontendOptions> frontendOptions)
    {
        _frontendOptions = frontendOptions.Value;
    }

    public string BuildResetPasswordLink(string email, string token)
    {
        var query = $"token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";

        return $"{_frontendOptions.BaseUrl}/reset-password?{query}";
    }
}
