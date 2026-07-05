using Application.Abstractions.Authentication;

namespace Presentation.Extensions;

public static class KeycloakExtensions
{
    public static async Task EnsureGitHubIdentityProviderAsync(this WebApplication app)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var provisioningService = scope.ServiceProvider.GetRequiredService<IIdentityProviderProvisioningService>();
            await provisioningService.EnsureGitHubIdentityProviderAsync();
            app.Logger.LogInformation("GitHub identity provider configured successfully");
        }
        catch (Exception exception)
        {
            app.Logger.LogError(exception, "An error occurred while configuring the GitHub identity provider");
            throw;
        }
    }
}
