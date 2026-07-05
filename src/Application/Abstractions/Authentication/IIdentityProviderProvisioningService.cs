namespace Application.Abstractions.Authentication;

public interface IIdentityProviderProvisioningService
{
    Task EnsureGitHubIdentityProviderAsync(CancellationToken cancellationToken = default);
}
